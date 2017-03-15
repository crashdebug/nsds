using System;
using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IModuleService : IDisposable
	{
		IEnumerable<Module> GetClientModules(Client client);
	}
}
