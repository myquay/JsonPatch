using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPatch.Tests.Entities;
using JsonPatch.Formatting;
using JsonPatch.Common.Paths.Resolvers;
using JsonPatch.Common.Paths.Components;

namespace JsonPatch.Tests
{
    [TestClass]
    public class FlexiblePathHelperTests
    {

        private FlexiblePathResolver resolver = new FlexiblePathResolver(new JsonValueConverter());

        [TestMethod]
        public void ParsePath_DataMember_ParsesSuccessfully()
        {
            //act
            var components = resolver.ParsePath("pId", typeof(SimpleEntity));

            //assert
            Assert.AreEqual(1, components.Length);
            Assert.AreEqual("ParId", components[0].Name);
            Assert.IsInstanceOfType(components[0], typeof(PropertyPathComponent));
            Assert.AreEqual(typeof(string), components[0].ComponentType);
        }

        [TestMethod]
        public void ParsePath_DataMember_PropertyNameFallback_ParsesSuccessfully()
        {
            //act
            var components = resolver.ParsePath("ParId", typeof(SimpleEntity));

            //assert
            Assert.AreEqual(1, components.Length);
            Assert.AreEqual("ParId", components[0].Name);
            Assert.IsInstanceOfType(components[0], typeof(PropertyPathComponent));
            Assert.AreEqual(typeof(string), components[0].ComponentType);
        }

        [TestMethod]
        public void ParsePath_DataMember_CaseInsensitivePropertyNameFallback_ParsesSuccessfully()
        {
            //act
            var components = resolver.ParsePath("car", typeof(SimpleEntity));

            //assert
            Assert.AreEqual(1, components.Length);
            Assert.AreEqual("Car", components[0].Name);
            Assert.IsInstanceOfType(components[0], typeof(PropertyPathComponent));
            Assert.AreEqual(typeof(string), components[0].ComponentType);
        }
    }
}
