using System;
using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IClientsStorage : IDisposable
	{
		IEnumerable<Client> GetClientsInPool(int poolId);
        void AddClient(Client client);
        IEnumerable<Client> GetAllClients();
		Client GetClient(string name);
		bool UpdateModuleVersion(Client client, Module module, BaseVersion version);
		ClientModule GetClientModule(string clientName, string moduleName);
	}
}
