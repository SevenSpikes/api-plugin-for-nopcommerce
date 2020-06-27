using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Constants;
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