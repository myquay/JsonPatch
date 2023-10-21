using JsonPatch.Paths.Components;
using JsonPatch.Paths.Resolvers;
using JsonPatch.Tests.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonPatch.Tests
{
    [TestClass]
    public class FlexiblePathHelperTests
    {
        private readonly FlexiblePathResolver resolver = new FlexiblePathResolver(new JsonValueConverter());

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
