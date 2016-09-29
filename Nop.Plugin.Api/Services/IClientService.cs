using System.Collections.Generic;
using Nop.Plugin.Api.Domain;

namespace Nop.Plugin.Api.Services
{
    public interface IClientService
    {
        bool ValidateClient(string clientId, string clientSecret, string authenticationCode);
        Client GetClient(string clientId);
        bool ValidateClientById(string clientId);
        IList<Client> GetAllClients();
        void DeleteClient(Client client);
        Client GetClientById(int id);
        Client GetClientByClientId(string clientId);

        void InsertClient(Client client);
        void UpdateClient(Client client);
    }
}