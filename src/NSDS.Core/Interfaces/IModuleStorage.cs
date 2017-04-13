using System;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IModuleStorage : IDisposable
	{
		Module GetModule(string name);
	}
}
