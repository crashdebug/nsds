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
		Client GetClient(int id);
		bool UpdateModuleVersion(Client client, Module module, BaseVersion version);
	}
}
