using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AutoMapper.Internal;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Factories;

namespace Nop.Plugin.Api.Helpers
{
    // TODO: Think of moving the mapping helper in teh delta folder
    public class MappingHelper : IMappingHelper
    {
        public void Merge(object source, object destination)
        {
            var sourcePropertyValuePairs = source.GetType()
                .GetProperties()
                .ToDictionary(property => property.Name, property => property.GetValue(source));

            SetValues(sourcePropertyValuePairs, destination, destination.GetType());
        }

        public void SetValues(Dictionary<string, object> jsonPropertiesValuePairsPassed, object objectToBeUpdated,
            Type propertyType)
        {
            foreach (var propertyValuePair in jsonPropertiesValuePairsPassed)
            {
                SetValue(objectToBeUpdated, propertyValuePair);
            }
        }

        // Used in the SetValue private method and also in the Delta.
        private void ConvertAndSetValueIfValid(object objectToBeUpdated, PropertyInfo objectProperty, object propertyValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(objectProperty.PropertyType);

            string propertyValueAsString = propertyValue.ToString();

            if (converter.IsValid(propertyValueAsString))
            {
                var convertedValue = converter.ConvertFrom(propertyValueAsString);

                objectProperty.SetValue(objectToBeUpdated, convertedValue);
            }
        }

        private void SetValue(object objectToBeUpdated, KeyValuePair<string, object> jsonPropertyValuePaires)
        {
            string propertyName = jsonPropertyValuePaires.Key;
            object propertyValue = jsonPropertyValuePaires.Value;

            PropertyInfo objectProperty = objectToBeUpdated.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            
            if (objectProperty != null)
            {
                // This case handles nested properties.
                if (propertyValue != null && propertyValue is Dictionary<string, object>)
                {
                    var valueToUpdate = objectProperty.GetValue(objectToBeUpdated);

                    if (valueToUpdate == null)
                    {
                        // Check if there is registered factory for this type.
                        Type factoryType = typeof (IFactory<>);
                        var factoryTypeForCurrentProperty = factoryType.MakeGenericType(new Type[] {objectProperty.PropertyType});
                        bool isFactoryRegistered = EngineContext.Current.ContainerManager.IsRegistered(factoryTypeForCurrentProperty);

                        if (isFactoryRegistered)
                        {
                            var initializerFactory = EngineContext.Current.Resolve(factoryTypeForCurrentProperty);
                            var initializeMethod = factoryTypeForCurrentProperty.GetMethod("Initialize");

                            valueToUpdate = initializeMethod.Invoke(initializerFactory, null);
                        }
                        else
                        {
                            valueToUpdate = Activator.CreateInstance(objectProperty.PropertyType);
                        }

                        objectProperty.SetValue(objectToBeUpdated, valueToUpdate);
                    }

                    // We need to use GetValue method to get the actual instance of the jsonProperty. objectProperty is the jsonProperty info.
                    SetValues((Dictionary<string, object>) propertyValue, valueToUpdate,
                        objectProperty.PropertyType);
                    // We expect the nested properties to be classes which are refrence types.
                    return;
                }
                // This case hadles collections.
                else if (propertyValue != null && propertyValue is ICollection<object>)
                {
                    ICollection<object> propertyValueAsCollection = propertyValue as ICollection<object>;

                    Type collectionElementsType = objectProperty.PropertyType.GetGenericArguments()[0];
                    var collection = objectProperty.GetValue(objectToBeUpdated);

                    if (collection == null)
                    {
                        var listType = typeof (List<>);
                        var constructedListType = listType.MakeGenericType(collectionElementsType);
                        collection = Activator.CreateInstance(constructedListType);
                        objectProperty.SetValue(objectToBeUpdated, collection);
                    }
                    
                    propertyValueAsCollection.Each(
                        x =>
                        {
                            if (collectionElementsType.Namespace != "System")
                            {
                                AddOrUpdateComplexItemInCollection(x as Dictionary<string, object>,
                                    collection as IList,
                                    collectionElementsType);
                            }
                            else
                            {
                                AddBaseItemInCollection(x, collection as IList, collectionElementsType);
                            }
                        });

                    return;
                }

                // This is where the new value is beeing set to the object jsonProperty using the SetValue function part of System.Reflection.
                if (propertyValue == null)
                {
                    objectProperty.SetValue(objectToBeUpdated, null);
                }
                else if (propertyValue is IConvertible)
                {
                    ConvertAndSetValueIfValid(objectToBeUpdated, objectProperty, propertyValue);
                    // otherwise ignore the passed value.
                }
                else
                {
                    objectProperty.SetValue(objectToBeUpdated, propertyValue);
                }
            }
        }

        private void AddBaseItemInCollection(object newItem, IList collection, Type collectionElementsType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(collectionElementsType);

            var newItemValueToString = newItem.ToString();

            if (converter.IsValid(newItemValueToString))
            {
                collection.Add(converter.ConvertFrom(newItemValueToString));
            }
        }

        private void AddOrUpdateComplexItemInCollection(Dictionary<string, object> newProperties, IList collection, Type collectionElementsType)
        {
            if (newProperties.ContainsKey("id"))
            {
                // Every element in collection, that is not System type should have an id.
                int id = int.Parse(newProperties["id"].ToString());

                object itemToBeUpdated = null;

                // Check if there is already an item with this id in the collection.
                foreach (var item in collection)
                {
                    if (int.Parse(item.GetType()
                        .GetProperty("id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        .GetValue(item)
                        .ToString()) == id)
                    {
                        itemToBeUpdated = item;
                        break;
                    }
                }

                if (itemToBeUpdated == null)
                {
                    // We should create a new item and put it in the collection.
                    AddNewItemInCollection(newProperties, collection, collectionElementsType);
                }
                else
                {
                    // We should update the existing element.
                    SetValues(newProperties, itemToBeUpdated, collectionElementsType);
                }
            }
            // It is a new item.
            else
            {
                AddNewItemInCollection(newProperties, collection, collectionElementsType);
            }
        }

        private void AddNewItemInCollection(Dictionary<string, object> newProperties, IList collection, Type collectionElementsType)
        {
            var newInstance = Activator.CreateInstance(collectionElementsType);

            var properties = collectionElementsType.GetProperties();

            SetEveryDatePropertyThatIsNotSetToDateTimeUtcNow(newProperties, properties);

            SetValues(newProperties, newInstance, collectionElementsType);

            collection.Add(newInstance);
        }

        // We need this method, because the default value of DateTime is not in the sql server DateTime range and we will get an exception if we use it.
        private void SetEveryDatePropertyThatIsNotSetToDateTimeUtcNow(Dictionary<string, object> newProperties, PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DateTime))
                {
                    bool keyFound = false;

                    // We need to loop through the keys, because the key may contain underscores in its name, which won't match the jsonProperty name.
                    foreach (var key in newProperties.Keys)
                    {
                        if (key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            keyFound = true;
                            break;
                        }
                    }

                    if (!keyFound)
                    {
                        // Create the item with the DateTime.NowUtc.
                        newProperties.Add(property.Name, DateTime.UtcNow);
                    }
                }
            }
        }
    }
}