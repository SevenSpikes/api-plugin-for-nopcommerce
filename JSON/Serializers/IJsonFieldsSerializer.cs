namespace Nop.Plugin.Api.JSON.Serializers
{
    using Nop.Plugin.Api.DTO;

    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
