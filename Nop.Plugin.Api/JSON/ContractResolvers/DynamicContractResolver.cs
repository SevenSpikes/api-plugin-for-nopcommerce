namespace Nop.Plugin.Api.JSON.ContractResolvers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class DynamicContractResolver : DefaultContractResolver
    {
        private Dictionary<string, bool> _propertiesToSerialize = null;

        public DynamicContractResolver(Dictionary<string, bool> propertiesToSerialize)
        {
            _propertiesToSerialize = propertiesToSerialize;
        }
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            string jsonPropertyNameLowerWithUnderscores = jsonProperty.PropertyName.ToLowerInvariant();
            string jsonPropertyNameWithoutUnderscores = jsonPropertyNameLowerWithUnderscores.Replace("_", string.Empty);

            // TODO: we should think if properties without underscore should be considered as valid ones
            // With this line we allow client not to follow the underscore convention - is this really necessary?
            jsonProperty.ShouldSerialize = o => _propertiesToSerialize.ContainsKey(jsonPropertyNameLowerWithUnderscores) || 
                                                _propertiesToSerialize.ContainsKey(jsonPropertyNameWithoutUnderscores);

            return jsonProperty;
        }
    }
}