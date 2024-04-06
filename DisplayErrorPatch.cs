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
			// ClO3Extension.InvokeTMLMethod("ModOrganizer", "DeleteMod", null, new object?[] { availableMods[i] });


			msg = "" +
				msg;
		}
		orig.Invoke(msg, e, fatal, continueIsRetry);
	}
}