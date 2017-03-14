using System;
using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IClientsService : IDisposable
	{
		IEnumerable<Client> GetClientsInPool(int poolId);
        Client AddClient(Client client);
        IEnumerable<Client> GetAllClients();
    }
}
