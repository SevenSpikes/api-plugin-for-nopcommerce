using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests
{
    [SetUpFixture]
    public class SetUp
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            // We need to create all the mappings before any of the test are run
            // All maps are created in the ApiMapperConfiguration constructor.
            ApiMapperConfiguration mapps = new ApiMapperConfiguration();
        }
    }
}
