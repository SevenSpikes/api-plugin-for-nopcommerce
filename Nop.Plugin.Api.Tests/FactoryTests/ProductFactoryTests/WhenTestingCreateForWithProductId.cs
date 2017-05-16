using AutoMock;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Factories;
using Nop.Services.Catalog;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.FactoryTests.ProductFactoryTests
{
    [TestFixture]
    public class WhenTestingCreateForWithProductId
    {
        private RhinoAutoMocker<ProductFactory> autoMocker;
        private ShoppingCartItemDto model;
        private IProductService _service;

        [SetUp]
        public void When()
        {
            autoMocker = new RhinoAutoMocker<ProductFactory>();
            model = new ShoppingCartItemDto() { ProductId = 123 };
            autoMocker.Get<IProductService>()
                .Stub(a => a.GetProductById(Arg<int>.Is.Anything))
                .Return(new Product() { Id = 444, Sku = "ABC"});

            _service = autoMocker.Get<IProductService>();
            var result = autoMocker.ClassUnderTest.CreateFor(model);
        }

       
        [Test]
        public void ItShouldCallGetProductById()
        {
            _service.AssertWasCalled(a => a.GetProductById(Arg<int>.Is.Anything));
        }

        [Test]
        public void ItShouldNotCallGetProductBySku()
        {
            _service.AssertWasNotCalled(a => a.GetProductBySku(Arg<string>.Is.Anything));
        }

        [Test]
        public void ItShouldSetProductSku()
        {
            Assert.AreEqual(model.ProductSku, "ABC");
        }
    }
}

