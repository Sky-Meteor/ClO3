using System;
using System.Reflection;
using Terraria.ModLoader;

namespace ClO3
{
	public class ClO3 : Mod
	{
		public static Type[] ModLoaderAssemblyTypes;

		//TODO:想办法让这个Mod早加载
		public override void Load()
		{
			ModLoaderAssemblyTypes = typeof(ModLoader).Assembly.GetTypes();
		}
	}
}