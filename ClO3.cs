using System;
using System.Reflection;
using Terraria.ModLoader;

namespace ClO3
{
	public class ClO3 : Mod
	{
		public static Type[] ModLoaderAssemblyTypes;

		//TODO:��취�����Mod�����
		public override void Load()
		{
			ModLoaderAssemblyTypes = typeof(ModLoader).Assembly.GetTypes();
		}
	}
}