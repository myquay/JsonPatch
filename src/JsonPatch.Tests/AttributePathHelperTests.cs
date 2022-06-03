using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPatch.Tests.Entitys;
using JsonPatch.Formatting;
using JsonPatch.Common.Paths.Resolvers;
using JsonPatch.Common.Paths.Components;

namespace JsonPatch.Tests
{
    [TestClass]
    public class AttributePathHelperTests
    {

        private IPathResolver resolver = new AttributePropertyPathResolver(new JsonValueConverter());

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
    }
}
