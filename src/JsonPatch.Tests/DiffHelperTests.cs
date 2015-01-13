using JsonPatch.Helpers;
using JsonPatch.Tests.Entitys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Tests
{
    [TestClass]
    public class DiffHelperTests
    {
        [TestMethod]
        public void GenerateDiff_SimpleEntityReplace_ReturnsReplaceOperation()
        {
            SimpleEntity originalDocument = new SimpleEntity() { Foo = "bar" };
            SimpleEntity modifiedDocument = new SimpleEntity() { Foo = "baz" };

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(typeof(SimpleEntity), originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.replace, operations.First().Operation);
            Assert.AreEqual("/Foo", operations.First().PropertyName);
            Assert.AreEqual(modifiedDocument.Foo, operations.First().Value);
        }

        [TestMethod]
        public void GenerateDiff_SimpleEntityRemove_ReturnsRemoveOperation()
        {
            SimpleEntity originalDocument = new SimpleEntity() { Foo = "bar" };
            SimpleEntity modifiedDocument = new SimpleEntity() { Foo = null };

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(typeof(SimpleEntity), originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.remove, operations.First().Operation);
            Assert.AreEqual("/Foo", operations.First().PropertyName);
            Assert.AreEqual(modifiedDocument.Foo, operations.First().Value);
        }
    }
}
