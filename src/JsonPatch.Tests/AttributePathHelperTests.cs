using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPatch.Paths;
using JsonPatch.Tests.Entitys;
using JsonPatch.Paths.Components;
using JsonPatch.Formatting;
using JsonPatch.Paths.Resolvers;

namespace JsonPatch.Tests
{
    [TestClass]
    public class AttributePathHelperTests
    {

        private AttributePropertyPathResolver _resolver;

        [TestInitialize]
        public void SetResolver()
        {
            _resolver = new AttributePropertyPathResolver();
            var settings = new JsonPatchSettings
            {
                PathResolver = _resolver
            };

            var formatter = new JsonPatchFormatter(settings);
        }

        [TestMethod]
        public void ParsePath_DataMember_ParsesSuccessfully()
        {
            //act
            var components = PathHelper.ParsePath("pId", typeof(SimpleEntity));

            //assert
            Assert.AreEqual(1, components.Length);
            Assert.AreEqual("ParId", components[0].Name);
            Assert.IsInstanceOfType(components[0], typeof(PropertyPathComponent));
            Assert.AreEqual(typeof(string), components[0].ComponentType);
        }

        [TestMethod]
        public void ParsePath_JsonProperty_ParsesSuccessfully()
        {
            //act
            var components = PathHelper.ParsePath("jsonProperty", typeof(SimpleEntity));

            //assert
            Assert.AreEqual(1, components.Length);
            Assert.AreEqual("Car", components[0].Name);
            Assert.IsInstanceOfType(components[0], typeof(PropertyPathComponent));
            Assert.AreEqual(typeof(string), components[0].ComponentType);
        }
    }
}
