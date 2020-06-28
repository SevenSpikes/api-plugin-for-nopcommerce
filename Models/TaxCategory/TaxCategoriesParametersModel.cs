using System.Collections.Generic;
using Newtonsoft.Json;
using static Nop.Plugin.Api.Infrastructure.Constants;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ProductsParameters
{
    using Microsoft.AspNetCore.Mvc;

    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<TaxCategoriesParametersModel>))]
    public class TaxCategoriesParametersModel : BaseProductsParametersModel
    {       
    }
}