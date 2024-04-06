using MonoMod.RuntimeDetour;
using Steamworks;
using System.IO;
using System;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.Social.Steam;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Version = System.Version;

namespace ClO3;

public class DisplayErrorPatch : ModSystem
{
	private delegate void DisplayLoadErrorDelegate(string msg, Exception e, bool fatal, bool continueIsRetry);

	private Hook _displayLoadErrorHook;

	public override void Load()
	{
		_displayLoadErrorHook = new Hook(typeof(ModLoader).GetMethod("DisplayLoadError", BindingFlags.NonPublic | BindingFlags.Instance), DisplayLoadError);
		_displayLoadErrorHook.Apply();
	}

	public override void Unload()
	{
		_displayLoadErrorHook.Dispose();
		_displayLoadErrorHook = null;
	}

	private static void DisplayLoadError(DisplayLoadErrorDelegate orig, string msg, Exception e, bool fatal, bool continueIsRetry)
	{
		if (e.Data.Contains("mod") || e.Data.Contains("mods"))
		{
			var responsibleMods = new List<string>();
			if (e.Data.Contains("mod"))
				responsibleMods.Add((string)e.Data["mod"]);
			if (e.Data.Contains("mods"))
				responsibleMods.AddRange((IEnumerable<string>)e.Data["mods"]);
			responsibleMods.Remove("ModLoader");

			string path = Path.GetFullPath(WorkshopHelper.GetWorkshopFolder(new AppId_t(1281930u)));
			path = Path.Combine(path, "content", "1281930");
			var availableMods = ClO3Extension.InvokeTMLMethod("ModOrganizer", "FindMods", null, new object?[] { false }) as IEnumerable<object>;
			var responsibleModsObj = new List<object>();
			if (availableMods != null)
			{
				foreach (object mod in availableMods)
				{
					if (mod.GetPropertyValue("Name") is not string modName || !responsibleMods.Contains(modName))
						continue;
					responsibleModsObj.Add(mod);
				}

				string modsStr = "";
				foreach (var mod in responsibleModsObj)
				{
					modsStr += (mod.GetPropertyValue("Name") as string) + (mod.GetPropertyValue("Version") as Version) + ", ";
				}

				string configPath = Path.Combine(Directory.GetParent(Main.SavePath).ToString(), "ModConfigs");
				int configCount = 0;
				foreach (string filePath in Directory.GetFiles(configPath))
				{
					if (responsibleMods.Contains(filePath.TakeWhile(c => c != '_')))
					{
						configCount++;
						// File.Delete(filePath);
					}
				}

				// ClO3Extension.InvokeTMLMethod("ModOrganizer", "DeleteMod", null, new object?[] { iii });
				
				modsStr = modsStr[..^2];

				msg = "你的tModLoader在加载中崩溃了，已为你找到相关Mod：\n" +
					  modsStr +
					  $"为你自动删除相关Mod及{configCount}个配置文件" +
					  msg;
			}

		}
		orig.Invoke(msg, e, fatal, continueIsRetry);
	}
}