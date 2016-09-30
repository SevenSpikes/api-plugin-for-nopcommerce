using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Models;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ClientMappings
    {
        public static ClientModel ToModel(this Client client)
        {
            return client.MapTo<Client, ClientModel>();
        }

        public static Client ToEntity(this ClientModel clientModel)
        {
            return clientModel.MapTo<ClientModel, Client>();
        }

        public static Client ToEntity(this ClientModel model, Client destination)
        {
            return model.MapTo<ClientModel, Client>(destination);
        }
    }
}