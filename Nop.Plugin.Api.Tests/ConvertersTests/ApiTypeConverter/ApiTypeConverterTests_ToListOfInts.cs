using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.Converters;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.ConvertersTests.ApiTypeConverter
{
    [TestFixture]
    public class ApiTypeConverterTests_ToListOfInts
    {
        private IApiTypeConverter _apiTypeConverter;

        [SetUp]
        public void SetUp()
        {
            _apiTypeConverter = new Converters.ApiTypeConverter();
        }

        [Test]
        [TestCase("a,b,c,d")]
        [TestCase(",")]
        [TestCase("invalid")]
        [TestCase("1 2 3 4 5")]
        [TestCase("&*^&^^*()_)_-1-=")]
        [TestCase("5756797879978978978978978978978978978, 234523523423423423423423423423423423423423")]
        public void WhenAllPartsOfTheListAreInvalid_ShouldReturnNull(string invalidList)
        {
            //Arange
           
            //Act
            IList<int> result = _apiTypeConverter.ToListOfInts(invalidList);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenNullOrEmptyStringPassed_ShouldReturnNull(string nullOrEmpty)
        {
            //Arange

            //Act
            IList<int> result = _apiTypeConverter.ToListOfInts(nullOrEmpty);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("1,2,3")]
        [TestCase("1, 4, 7")]
        [TestCase("0,-1, 7, 9 ")]
        [TestCase("   0,1  , 7, 9   ")]
        public void WhenValidListPassed_ShouldReturnThatList(string validList)
        {
            //Arange
            List<int> expectedList = validList.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            //Act
            IList<int> result = _apiTypeConverter.ToListOfInts(validList);

            //Assert
            CollectionAssert.AreEqual(expectedList, result);
        }

        [Test]
        [TestCase("1,2, u,3")]
        [TestCase("a, b, c, 1")]
        [TestCase("0,-1, -, 7, 9 ")]
        [TestCase("%^#^^,$,#,%,8")]
        [TestCase("0")]
        [TestCase("097")]
        [TestCase("087, 05667, sdf")]
        [TestCase("017, 345df, 05867")]
        [TestCase("67856756, 05867, 76767ergdf")]
        [TestCase("690, 678678678678678678678678678678678678676867867")]
        public void WhenSomeOfTheItemsAreValid_ShouldReturnThatListContainingOnlyTheValidItems(string mixedList)
        {
            //Arange
            List<int> expectedList = new List<int>();
            var collectionSplited = mixedList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int tempInt;
            foreach (var item in collectionSplited)
            {
                if (int.TryParse(item, out tempInt))
                {
                    expectedList.Add(tempInt);
                }
            }

            //Act
            IList<int> result = _apiTypeConverter.ToListOfInts(mixedList);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AreEqual(expectedList, result);
        }

        [Test]
        [TestCase("f,d, u,3")]
        [TestCase("0")]
        [TestCase("097")]
        [TestCase("67856756, 05ert867, 76767ergdf")]
        [TestCase("690, 678678678678678678678678678678678678676867867")]
        public void WhenOnlyOneOfTheItemsIsValid_ShouldReturnListContainingOnlyThatItem(string mixedList)
        {
            //Arange
            List<int> expectedList = new List<int>();
            var collectionSplited = mixedList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int tempInt;
            foreach (var item in collectionSplited)
            {
                if (int.TryParse(item, out tempInt))
                {
                    expectedList.Add(tempInt);
                }
            }

            //Act
            IList<int> result = _apiTypeConverter.ToListOfInts(mixedList);

            //Assert
            Assert.AreEqual(1, result.Count);
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AreEqual(expectedList, result);
        }
    }
}