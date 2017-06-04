using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using AutoMapper.Configuration;
using Newtonsoft.Json;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class TypeValidator<T>
    {
        public List<string> InvalidProperties { get; set; }

        public TypeValidator()
        {
            InvalidProperties = new List<string>();
        }

        public bool IsValid(Dictionary<string, object> propertyValuePaires)
        {
            bool isValid = true;

            var jsonPropertyNameTypePair = new Dictionary<string, Type>();

            var objectProperties = typeof(T).GetProperties();

            foreach (var property in objectProperties)
            {
                JsonPropertyAttribute jsonPropertyAttribute = property.GetCustomAttribute(typeof(JsonPropertyAttribute)) as JsonPropertyAttribute;

                if (jsonPropertyAttribute != null)
                {
                    if (!jsonPropertyNameTypePair.ContainsKey(jsonPropertyAttribute.PropertyName))
                    {
                        jsonPropertyNameTypePair.Add(jsonPropertyAttribute.PropertyName, property.PropertyType);
                    }
                }
            }

            foreach (var pair in propertyValuePaires)
            {
                bool isCurrentPropertyValid = true;

                if (jsonPropertyNameTypePair.ContainsKey(pair.Key))
                {
                    var propertyType = jsonPropertyNameTypePair[pair.Key];

                    // handle nested properties
                    if (pair.Value is Dictionary<string, object>)
                    {
                        isCurrentPropertyValid = ValidateNestedProperty(propertyType, (Dictionary<string, object>)pair.Value);
                    }
                    // This case hadles collections.
                    else if (pair.Value != null && pair.Value is ICollection<object> &&
                        propertyType.GetInterface("IEnumerable") != null)
                    {
                        var elementsType = ReflectionHelper.GetGenericElementType(propertyType);

                        ICollection<object> propertyValueAsCollection = pair.Value as ICollection<object>;

                        // Validate the collection items.
                        foreach (var item in propertyValueAsCollection)
                        {
                            isCurrentPropertyValid = IsCurrentPropertyValid(elementsType, item);

                            if (!isCurrentPropertyValid) break;
                        }
                    }
                    else
                    {
                        isCurrentPropertyValid = IsCurrentPropertyValid(jsonPropertyNameTypePair[pair.Key], pair.Value);
                    }

                    if (!isCurrentPropertyValid)
                    {
                        isValid = false;
                        InvalidProperties.Add(pair.Key);
                    }
                }
            }

            return isValid;
        }

        private bool ValidateNestedProperty(Type propertyType, Dictionary<string, object> value)
        {
            bool isCurrentPropertyValid = true;

            Type constructedType = typeof(TypeValidator<>).MakeGenericType(propertyType);
            var typeValidatorForNestedProperty = Activator.CreateInstance(constructedType);

            var isValidMethod = constructedType.GetMethod("IsValid");

            isCurrentPropertyValid = (bool)isValidMethod.Invoke(typeValidatorForNestedProperty, new object[] { value });

            return isCurrentPropertyValid;
        }

        private bool IsCurrentPropertyValid(Type type, object value)
        {
            bool isCurrentPropertyValid = true;

            if (type.Namespace == "System")
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);

                var valueToValidate = value;

                // This is needed because the isValid method does not work well if the value it is trying to validate is object.
                if (value != null)
                {
                    valueToValidate = string.Format(CultureInfo.InvariantCulture, "{0}", value);
                }

                if (!converter.IsValid(valueToValidate)) isCurrentPropertyValid = false;
            }
            else
            {
                if (value != null)
                {
                    isCurrentPropertyValid = ValidateNestedProperty(type, (Dictionary<string, object>)value);
                }
            }

            return isCurrentPropertyValid;
        }
    }
}