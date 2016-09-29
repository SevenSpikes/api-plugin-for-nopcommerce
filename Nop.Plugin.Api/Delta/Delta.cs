using System;
using System.Collections;
using System.Collections.Generic;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Maps;

namespace Nop.Plugin.Api.Delta
{
    public class Delta<TDto> where TDto : class, new()
    {
        private TDto _dto;

        private readonly IMappingHelper _mappingHelper = new MappingHelper();

        private readonly Dictionary<string, object> _changedJsonPropertyNames;
        
        private readonly IJsonPropertyMapper _jsonPropertyMapper;

        private Dictionary<string, object> _propertyValuePairs; 

        private Dictionary<string, object> PropertyValuePairs
        {
            get
            {
                if (_propertyValuePairs == null)
                {
                    _propertyValuePairs = GetPropertyValuePairs(typeof(TDto), _changedJsonPropertyNames);
                }

                return _propertyValuePairs;
            }
        } 

        public TDto Dto
        {
            get
            {
                if (_dto == null)
                {
                    _dto = new TDto();
                }

                return _dto;
            }
        }

        public Delta(Dictionary<string, object> passedChangedJsonPropertyValuePaires)
        {
            _jsonPropertyMapper = EngineContext.Current.Resolve<IJsonPropertyMapper>();
            _changedJsonPropertyNames = passedChangedJsonPropertyValuePaires;

            _mappingHelper.SetValues(PropertyValuePairs, Dto, typeof(TDto));
        }

        public void Merge<TEntity>(TEntity entity)
        {
            _mappingHelper.SetValues(PropertyValuePairs, entity, entity.GetType());
        }

        private Dictionary<string, object> GetPropertyValuePairs(Type type, Dictionary<string, object> changedJsonPropertyNames)
        {
            var propertyValuePairs = new Dictionary<string, object>();

            Dictionary<string, Tuple<string, Type>> typeMap = _jsonPropertyMapper.GetMap(type);

            foreach (var changedProperty in changedJsonPropertyNames)
            {
                string jsonName = changedProperty.Key;

                if (typeMap.ContainsKey(jsonName))
                {
                    Tuple<string, Type> propertyNameAndType = typeMap[jsonName];

                    string propertyName = propertyNameAndType.Item1;
                    Type propertyType = propertyNameAndType.Item2;

                    // Handle system types
                    // This is also the recursion base
                    if (propertyType.Namespace == "System")
                    {
                        propertyValuePairs.Add(propertyName, changedProperty.Value);
                    }
                    // Handle collections
                    else if (propertyType.GetInterface(typeof(IEnumerable).FullName) != null)
                    {
                        var collection = changedProperty.Value as IEnumerable<object>;
                        Type collectionElementsType = propertyType.GetGenericArguments()[0];
                        var resultCollection = new List<object>();

                        foreach (var item in collection)
                        {
                            // Simple types in collection
                            if (collectionElementsType.Namespace == "System")
                            {
                                resultCollection.Add(item);
                            }
                            // Complex types in collection
                            else
                            {
                                resultCollection.Add(GetPropertyValuePairs(collectionElementsType,
                                    (Dictionary<string, object>) item));
                            }
                        }

                        propertyValuePairs.Add(propertyName, resultCollection);
                    }
                    // Handle nested properties
                    else
                    {
                        var resultedNestedObject = GetPropertyValuePairs(propertyType, (Dictionary<string, object>)changedProperty.Value);

                        propertyValuePairs.Add(propertyName, resultedNestedObject);
                    }
                }
            }

            return propertyValuePairs;
        }
    }
}