using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication
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
				else if (type.IsAssignableFrom(t))
				{
					coll.AddTransient(type, t);
				}
			}
		}
	}
}
