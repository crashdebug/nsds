using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NSDS.Core
{
	public static class IServiceCollectionExtensions
	{
		public static void RegisterTypes(this IServiceCollection coll, Type type, Assembly assembly = null)
		{
			if (assembly == null)
			{
				assembly = Assembly.GetEntryAssembly();
			}
			foreach (var t in assembly.GetTypes())
			{
				if (t == type)
				{
					continue;
				}
				else if (type.GetTypeInfo().IsSubclassOf(t))
				{
					coll.AddTransient(type, t);
				}
			}
		}
	}
}
