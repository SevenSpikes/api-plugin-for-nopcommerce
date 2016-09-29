using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.ProductCategoryMappings.GetMappingById
{
    [TestFixture]
    public class ProductCategoryMappingsApiServiceTests_GetById
    {
        [Test]
        public void WhenNullIsReturnedByTheRepository_ShouldReturnNull()
        {
            int mappingId = 3;
            
            // Arange
            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            productCategoryRepo.Stub(x => x.GetById(mappingId)).Return(null);
            
            // Act  
            var cut = new ProductCategoryMappingsApiService(productCategoryRepo);
            var result = cut.GetById(mappingId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(-2)]
        [TestCase(0)]
        public void WhenNegativeOrZeroMappingIdPassed_ShouldReturnNull(int negativeOrZeroOrderId)
        {
            // Aranges
            var mappingRepoMock = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepoMock);
            var result = cut.GetById(negativeOrZeroOrderId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void WhenMappingIsReturnedByTheRepository_ShouldReturnTheSameMapping()
        {
            int mappingId = 3;
            var mapping = new ProductCategory() { Id = 3 };

            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.GetById(mappingId)).Return(mapping);
            
            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);
            var result = cut.GetById(mappingId);

            // Assert
            Assert.AreSame(mapping, result);
        }
    }
}
