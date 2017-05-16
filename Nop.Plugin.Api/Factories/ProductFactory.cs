using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Services.Catalog;
using Nop.Services.Directory;

namespace Nop.Plugin.Api.Factories
{
    public class ProductFactory : IFactory<Product>, IShoppingCartFactory<Product>
    {
        private readonly IMeasureService _measureService;
        private readonly IMeasureSettings _measureSettings;

        private readonly IProductService _productService;

        public ProductFactory(IMeasureService measureService, IMeasureSettings measureSettings, IProductService productService)
        {
            _measureService = measureService;
            _measureSettings = measureSettings;
            _productService = productService;
        }

        public Product Initialize()
        {
            var defaultProduct = new Product();
            
            defaultProduct.Weight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Ratio;

            defaultProduct.CreatedOnUtc = DateTime.UtcNow;
            defaultProduct.UpdatedOnUtc = DateTime.UtcNow;

            defaultProduct.ProductType = ProductType.SimpleProduct;

            defaultProduct.MaximumCustomerEnteredPrice = 1000;
            defaultProduct.MaxNumberOfDownloads = 10;
            defaultProduct.RecurringCycleLength = 100;
            defaultProduct.RecurringTotalCycles = 10;
            defaultProduct.RentalPriceLength = 1;
            defaultProduct.StockQuantity = 10000;
            defaultProduct.NotifyAdminForQuantityBelow = 1;
            defaultProduct.OrderMinimumQuantity = 1;
            defaultProduct.OrderMaximumQuantity = 10000;
            
            defaultProduct.UnlimitedDownloads = true;
            defaultProduct.IsShipEnabled = true;
            defaultProduct.AllowCustomerReviews = true;
            defaultProduct.Published = true;
            defaultProduct.VisibleIndividually = true;
           
            return defaultProduct;
        }

        public Product CreateFor(ShoppingCartItemDto model)
        {
            Product product;
            if (model.ProductId.HasValue)
            {
               product= _productService.GetProductById(model.ProductId.Value);
               model.ProductSku = product.Sku;
            }
            else
            {
                product = _productService.GetProductBySku(model.ProductSku);
                model.ProductId = product.Id;
            }
            return product;
        }
    }
}