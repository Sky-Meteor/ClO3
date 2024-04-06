using System;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace ClO3;

public static class ClO3Extension
{
	public static Type GetTMLType(string typeName) => ClO3.ModLoaderAssemblyTypes.First(t => t.Name == typeName);

	public static object? InvokeMethod(this Type type, string name, object? invokeObj, object?[]? parameters = null) => 
		type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Invoke(invokeObj, parameters);

	public static object? InvokeTMLMethod(string typeName, string methodName, object? invokeObj, object?[]? parameters = null) =>
		GetTMLType(typeName).InvokeMethod(methodName, invokeObj, parameters);


}