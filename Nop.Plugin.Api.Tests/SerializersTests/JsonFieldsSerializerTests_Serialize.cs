using System;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.Tests.SerializersTests.DummyObjects;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.SerializersTests
{
    using Nop.Plugin.Api.JSON.Serializers;

    [TestFixture]
    public class JsonFieldsSerializerTests_Serialize
    {
        
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenEmptyFieldsParameterPassed_ShouldSerializeEverythingFromThePassedObject(string emptyFieldsParameter)
        {
            //Arange
            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            var serializableObject = new SerializableDummyObjectWithSimpleTypes();
            serializableObject.Items.Add(new DummyObjectWithSimpleTypes()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value"
            });

            //Act
            string serializedObjectJson = cut.Serialize(serializableObject, emptyFieldsParameter);

            //Assert 
            SerializableDummyObjectWithSimpleTypes dummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<SerializableDummyObjectWithSimpleTypes>(serializedObjectJson);

            Assert.AreEqual(serializableObject.Items.Count, dummySerializableObjectFromJson.Items.Count);
            Assert.AreEqual(serializableObject.Items[0], dummySerializableObjectFromJson.Items[0]);
            Assert.AreEqual("first property value", dummySerializableObjectFromJson.Items[0].FirstProperty);
            Assert.AreEqual("second property value", dummySerializableObjectFromJson.Items[0].SecondProperty);
        }
        
        [Test]
        public void WhenNullObjectToSerializePassed_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(WhenNulObjectToSearializePassed);
        }

        private void WhenNulObjectToSearializePassed()
        {
            //Arange
            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            //Act
            cut.Serialize(Arg<ISerializableObject>.Is.Null, Arg<string>.Is.Anything);
        }

        [Test]
        public void WhenNoValidFieldsInTheFieldsParameterPassed_ShouldReturnEmptyCollectionJson()
        {
            var dummySerializableObject = new SerializableDummyObjectWithSimpleTypes();
            dummySerializableObject.Items.Add(new DummyObjectWithSimpleTypes()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });


            // Arange
            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            // Act
            string json = cut.Serialize(dummySerializableObject, "not valid fields");

            // Assert
            SerializableDummyObjectWithSimpleTypes dummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<SerializableDummyObjectWithSimpleTypes>(json);

            Assert.AreEqual(0, dummySerializableObjectFromJson.Items.Count);
        }

        [Test]
        public void WhenValidFieldsParameterPassed_ShouldSerializeTheseFieldsJson()
        {
            var dummySerializableObject = new SerializableDummyObjectWithSimpleTypes();
            dummySerializableObject.Items.Add(new DummyObjectWithSimpleTypes()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });

            // Arange
            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            // Act
            string json = cut.Serialize(dummySerializableObject, "first_property");

            // Assert
            SerializableDummyObjectWithSimpleTypes dummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<SerializableDummyObjectWithSimpleTypes>(json);

            Assert.AreEqual(1, dummySerializableObjectFromJson.Items.Count);
            Assert.AreEqual("first property value", dummySerializableObjectFromJson.Items[0].FirstProperty);
        }

        [Test]
        public void WhenValidFieldsParameterPassed_ShouldNotSerializeOtherFieldsJson()
        {
            var dummySerializableObject = new SerializableDummyObjectWithSimpleTypes();
            dummySerializableObject.Items.Add(new DummyObjectWithSimpleTypes()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });

            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            // Act
            string json = cut.Serialize(dummySerializableObject, "first_property");

            // Assert
            SerializableDummyObjectWithSimpleTypes dummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<SerializableDummyObjectWithSimpleTypes>(json);

            Assert.AreEqual(1, dummySerializableObjectFromJson.Items.Count);
            Assert.IsNull(dummySerializableObjectFromJson.Items[0].SecondProperty);
        }

        /* Complex dummy object */

        [Test]
        public void WhenValidFieldsParameterPassed_ShouldSerializeTheseFieldsJson_ComplexDummyObject()
        {
            var complexDummySerializableObject = new SerializableDummyObjectWithComplexTypes();

            complexDummySerializableObject.Items.Add(new DummyObjectWithComplexTypes
            {
                StringProperty = "string value",
                DummyObjectWithSimpleTypes = new DummyObjectWithSimpleTypes
                {
                    FirstProperty = "first property value",
                    SecondProperty = "second property value"
                },
                ListOfDummyObjectWithSimpleTypes = new List<DummyObjectWithSimpleTypes>
                {
                    new DummyObjectWithSimpleTypes()
                    {
                        FirstProperty = "first property of list value",
                        SecondProperty = "second property of list value"
                    }
                }
            });

            // Arange
            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            // Act
            string json = cut.Serialize(complexDummySerializableObject, "string_property, dummy_object_with_simple_types, list_of_dummy_object_with_simple_types");

            // Assert
            SerializableDummyObjectWithComplexTypes complexDummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<SerializableDummyObjectWithComplexTypes>(json);

            Assert.AreEqual(1, complexDummySerializableObjectFromJson.Items.Count);
            Assert.AreEqual(1, complexDummySerializableObjectFromJson.Items[0].ListOfDummyObjectWithSimpleTypes.Count);
            Assert.AreEqual("string value", complexDummySerializableObjectFromJson.Items[0].StringProperty);
        }

        [Test]
        public void WhenSecondLevelValidFieldsParameterPassed_ShouldSerializeEmptyJson_ComplexDummyObject()
        {
            var complexDummySerializableObject = new SerializableDummyObjectWithComplexTypes();

            complexDummySerializableObject.Items.Add(new DummyObjectWithComplexTypes
            {
                StringProperty = "string value",
                DummyObjectWithSimpleTypes = new DummyObjectWithSimpleTypes
                {
                    FirstProperty = "first property value",
                    SecondProperty = "second property value"
                },
                ListOfDummyObjectWithSimpleTypes = new List<DummyObjectWithSimpleTypes>
                {
                    new DummyObjectWithSimpleTypes()
                    {
                        FirstProperty = "first property of list value",
                        SecondProperty = "second property of list value"
                    }
                }
            });

            // Arange
            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            // Act
            string json = cut.Serialize(complexDummySerializableObject, "first_property");

            // Assert
            SerializableDummyObjectWithComplexTypes complexDummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<SerializableDummyObjectWithComplexTypes>(json);

            Assert.AreEqual(0, complexDummySerializableObjectFromJson.Items.Count);
        }

        [Test]
        public void WhenValidFieldsParameterPassed_ShouldSerializeTheseFieldsJson_ComplexDummyObjectEmptyList()
        {
            var complexDummySerializableObject = new SerializableDummyObjectWithComplexTypes();

            complexDummySerializableObject.Items.Add(new DummyObjectWithComplexTypes
            {
                ListOfDummyObjectWithSimpleTypes = new List<DummyObjectWithSimpleTypes>()
            });

            // Arange
            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            // Act
            string json = cut.Serialize(complexDummySerializableObject, "list_of_dummy_object_with_simple_types");

            // Assert
            SerializableDummyObjectWithComplexTypes complexDummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<SerializableDummyObjectWithComplexTypes>(json);

            Assert.AreEqual(1, complexDummySerializableObjectFromJson.Items.Count);
            Assert.AreEqual(0, complexDummySerializableObjectFromJson.Items[0].ListOfDummyObjectWithSimpleTypes.Count);
        }
        
        [Test]
        public void WhenInValidFieldsParameterPassed_ShouldSerializeTheseFieldsJson_ComplexDummyObject()
        {
            var complexDummySerializableObject = new SerializableDummyObjectWithComplexTypes();

            complexDummySerializableObject.Items.Add(new DummyObjectWithComplexTypes
            {
                StringProperty = "string value",
                DummyObjectWithSimpleTypes = new DummyObjectWithSimpleTypes
                {
                    FirstProperty = "first property value",
                    SecondProperty = "second property value"
                },
                ListOfDummyObjectWithSimpleTypes = new List<DummyObjectWithSimpleTypes>
                {
                    new DummyObjectWithSimpleTypes()
                    {
                        FirstProperty = "first property of list value",
                        SecondProperty = "second property of list value"
                    }
                }
            });

            // Arange
            IJsonFieldsSerializer cut = new JsonFieldsSerializer();

            // Act
            string json = cut.Serialize(complexDummySerializableObject, "invalid field");

            // Assert
            SerializableDummyObjectWithComplexTypes complexDummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<SerializableDummyObjectWithComplexTypes>(json);

            Assert.AreEqual(0, complexDummySerializableObjectFromJson.Items.Count);
        }
    }
}