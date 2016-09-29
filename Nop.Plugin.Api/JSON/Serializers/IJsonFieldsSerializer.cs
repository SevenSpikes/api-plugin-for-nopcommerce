using Nop.Plugin.Api.DTOs;

namespace Nop.Plugin.Api.Serializers
{
    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
