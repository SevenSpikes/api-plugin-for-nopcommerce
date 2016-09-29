using AutoMapper;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Models;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ConfigurationMappings
    {
        public static ConfigurationModel ToModel(this ApiSettings apiSettings)
        {
            return Mapper.Map<ApiSettings, ConfigurationModel>(apiSettings);
        }

        public static ApiSettings ToEntity(this ConfigurationModel apiSettingsModel)
        {
            return Mapper.Map<ConfigurationModel, ApiSettings>(apiSettingsModel);
        }
    }
}