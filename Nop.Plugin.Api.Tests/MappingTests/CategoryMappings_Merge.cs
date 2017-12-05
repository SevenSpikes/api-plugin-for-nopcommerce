using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.MappingTests
{
    [TestFixture]
    public class CategoryMappings_Merge
    {
        // This attribute means that the setup method will be called only once for all tests, not before each test.
        //[OneTimeSetUp]
        //public void Setup()
        //{
        //    Maps.CreateUpdateMap<CategoryDto, Category>();
        //}

        //[Test]
        //public void WhenOnlyTheNamePropertyIsSetInDto_GivenEntityWithMultiplePropertiesSetIncludingTheName_ShouldReturnEntityWithOnlyTheNameChanged()
        //{
        //    var entity = new Category()
        //    {
        //        Name = "entity name",
        //        Description = "some description"
        //    };

        //    var dto = new CategoryDto()
        //    {
        //        Name = "updated name"
        //    };

        //    //Attributes are readonly - http://stackoverflow.com/questions/10046601/change-custom-attributes-parameter-at-runtime
        //    // So we are going to try to replace the attribute with new instance.

        //    //******************************
            
        //    PropertyOverridingTypeDescriptor ctd = new PropertyOverridingTypeDescriptor(TypeDescriptor.GetProvider(dto).GetTypeDescriptor(dto));

        //    PropertyDescriptor desriptionPorpertyDescriptor = ctd.GetProperties().Find("Description", true);

        //    PropertyDescriptor pd2 =
        //    TypeDescriptor.CreateProperty(
        //        dto.GetType(),
        //        desriptionPorpertyDescriptor, // base property descriptor to which we want to add attributes
        //            // The PropertyDescriptor which we'll get will just wrap that
        //            // base one returning attributes we need.
        //        new Updated(false)
        //    // this method really can take as many attributes as you like,
        //    // not just one
        //    );

        //    // and then we tell our new PropertyOverridingTypeDescriptor to override that property
        //    ctd.OverrideProperty(pd2);

        //    // then we add new descriptor provider that will return our descriptor istead of default
        //    TypeDescriptor.AddProvider(new TypeDescriptorOverridingProvider(ctd), dto);
            
        //    //******************************

        //    Category resultEntity = entity.Merge(dto);

        //    // The name should be updated
        //    Assert.AreEqual(dto.Name, resultEntity.Name);
        //    // The description shouldn't be updated
        //    Assert.NotNull(entity.Description);
        //    Assert.AreEqual(entity.Description, resultEntity.Description);
        //}
    }
}