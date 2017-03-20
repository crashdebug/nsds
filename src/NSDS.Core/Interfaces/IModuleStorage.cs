﻿using System;
using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IModuleStorage : IDisposable
	{
		IEnumerable<Module> GetClientModules(Client client);
		Module GetModule(int moduleId);
	}
}
