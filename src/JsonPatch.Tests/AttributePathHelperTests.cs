using JsonPatch.Paths.Components;
using JsonPatch.Paths.Resolvers;
using JsonPatch.Tests.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonPatch.Tests
{
    [TestClass]
    public class AttributePathHelperTests
    {
        private readonly IPathResolver resolver = new AttributePropertyPathResolver(new JsonValueConverter());

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
