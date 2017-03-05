using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.Interfaces
{
	public interface IClientsService
	{
		IEnumerable<Client> GetClientsInPool(int poolId);
        void AddClient(Client client);
        IEnumerable<Client> GetAllClients();
    }
}
