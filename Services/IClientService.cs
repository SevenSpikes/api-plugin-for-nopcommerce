using System.Collections.Generic;

namespace Nop.Plugin.Api.Services
{
    using Models;
    using Nop.Web.Framework.Models;

    public interface IClientService
    {
        ClientApiListModel GetAllClients(ClientApiSearchModel model);
        void DeleteClient(int id);
        int InsertClient(ClientApiModel model);
        void UpdateClient(ClientApiModel model);
        ClientApiModel FindClientByIdAsync(int id);
        ClientApiModel FindClientByClientId(string clientId);
    }
}