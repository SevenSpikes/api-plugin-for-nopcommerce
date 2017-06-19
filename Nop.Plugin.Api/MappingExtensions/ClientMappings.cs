using AutoMapper;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Models;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ClientMappings
    {
        public static ClientModel ToModel(this Client client)
        {
            return Mapper.Map<Client, ClientModel>(client);
        }

        public static Client ToEntity(this ClientModel clientModel)
        {
            return Mapper.Map<ClientModel, Client>(clientModel);
        }

        public static Client ToEntity(this ClientModel model, Client destination)
        {
            return Mapper.Map(model, destination);
        }
    }
}