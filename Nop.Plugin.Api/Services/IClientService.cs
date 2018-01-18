using System.Collections.Generic;

namespace Nop.Plugin.Api.Services
{
    using Nop.Plugin.Api.Models;

    public interface IClientService
    {
        IList<ClientApiModel> GetAllClients();
        void DeleteClient(int id);
        int InsertClient(ClientApiModel model);
        void UpdateClient(ClientApiModel model);
        ClientApiModel FindClientByIdAsync(int id);
        ClientApiModel FindClientByClientId(string clientId);
    }
}