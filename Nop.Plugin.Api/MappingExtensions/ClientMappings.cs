using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.Domain;

namespace Nop.Plugin.Api.MappingExtensions
{
    using Nop.Plugin.Api.Models;

    public static class ClientMappings
    {
        public static ClientApiModel ToModel(this Client client)
        {
            return client.MapTo<Client, ClientApiModel>();
        }

        public static Client ToEntity(this ClientApiModel clientModel)
        {
            return clientModel.MapTo<ClientApiModel, Client>();
        }

        public static Client ToEntity(this ClientApiModel model, Client destination)
        {
            return model.MapTo<ClientApiModel, Client>(destination);
        }
    }
}