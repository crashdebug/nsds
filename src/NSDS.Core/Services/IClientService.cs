using System.Collections.Generic;
using NSDS.Core.Models;

namespace NSDS.Core.Interfaces
{
	public interface IClientsService
	{
		IEnumerable<Client> GetClientsInPool(int poolId);
        void AddClient(Client client);
        IEnumerable<Client> GetAllClients();
    }
}
