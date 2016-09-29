using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Api.Domain;

namespace Nop.Plugin.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _clientRepository;
        public ClientService(IRepository<Client> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public bool ValidateClient(string clientId, string clientSecret, string authenticationCode)
        {
            return _clientRepository.Table.Any(client => client.ClientId == clientId &&
                                                         client.ClientSecret == clientSecret &&
                                                         client.AuthenticationCode == authenticationCode);
        }

        public Client GetClient(string clientId)
        {
            return _clientRepository.Table.FirstOrDefault(client => client.ClientId == clientId);
        }

        public bool ValidateClientById(string clientId)
        {
            return _clientRepository.Table.Any(client => client.ClientId == clientId);
        }

        public IList<Client> GetAllClients()
        {
            return _clientRepository.Table.ToList();
        }

        public Client GetClientById(int id)
        {
            return _clientRepository.GetById(id);
        }

        public Client GetClientByClientId(string clientId)
        {
            return _clientRepository.Table.FirstOrDefault(client => client.ClientId == clientId);
        }

        public void InsertClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _clientRepository.Insert(client);
        }

        public void UpdateClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _clientRepository.Update(client);
        }

        public void DeleteClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _clientRepository.Delete(client);
        }
    }
}