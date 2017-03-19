using System;
using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IDeploymentStorage : IDisposable
	{
		IEnumerable<Deployment> GetDeployments();
	}
}
