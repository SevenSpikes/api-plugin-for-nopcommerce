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
    public class WhenTestingCreateForWithProductSku
    {
        private RhinoAutoMocker<ProductFactory> autoMocker;
        private ShoppingCartItemDto model;
        private IProductService _service;

        [SetUp]
        public void When()
        {
            autoMocker = new RhinoAutoMocker<ProductFactory>();
            model = new ShoppingCartItemDto() { ProductSku = "osos" };
            autoMocker.Get<IProductService>()
                .Stub(a => a.GetProductBySku(Arg<string>.Is.Anything))
                .Return(new Product() { Id = 444, Sku = "ABC"});

            _service = autoMocker.Get<IProductService>();
            var result = autoMocker.ClassUnderTest.CreateFor(model);
        }

       
        [Test]
        public void ItShouldNotCallGetProductById()
        {
            _service.AssertWasNotCalled(a => a.GetProductById(Arg<int>.Is.Anything));
        }

        [Test]
        public void ItShouldCallGetProductBySku()
        {
            _service.AssertWasCalled(a => a.GetProductBySku(Arg<string>.Is.Anything));
        }

        [Test]
        public void ItShouldSetProductId()
        {
            Assert.AreEqual(model.ProductId, 444);
        }
    }
}

