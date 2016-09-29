using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests
{
    //[TestFixture]
    //public class CountryApiServiceTests
    //{
    //    private ICountryApiService _countryApiService;

    //    [SetUp]
    //    public new void SetUp()
    //    {
    //        var countryRepositoryStub = MockRepository.GenerateStub<IRepository<Country>>();

    //        countryRepositoryStub.Stub(x => x.Table).Return((new List<Country>()
    //        {

    //            new Country()
    //            {
    //                Name = "test country 1"
    //            },
    //            new Country()
    //            {
    //                Name = "test country 2"
    //            }

    //        }).AsQueryable());

    //        _countryApiService = new CountryApiService(countryRepositoryStub);
    //    }

    //    [Test]
    //    public void Get_country_by_existing_name()
    //    {
    //        var countryResult = _countryApiService.GetCountryByName("test country 1");

    //        Assert.IsNotNull(countryResult);
    //        Assert.AreEqual("test country 1", countryResult.Name);
    //    }

    //    [Test]
    //    public void Get_country_by_non_existing_name()
    //    {
    //        var countryResult = _countryApiService.GetCountryByName("non existing country name");

    //        Assert.IsNull(countryResult);
    //    }
    //}
}