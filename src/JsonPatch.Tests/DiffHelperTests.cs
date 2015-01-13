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

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

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

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.remove, operations.First().Operation);
            Assert.AreEqual("/Foo", operations.First().PropertyName);
            Assert.AreEqual(modifiedDocument.Foo, operations.First().Value);
        }

        [TestMethod]
        public void GenerateDiff_NestedEntityReplace_ReturnsReplaceOperation()
        {
            NestedEntity originalDocument = new NestedEntity();
            originalDocument.Foo = new SimpleEntity() { Foo = "bar" };
            NestedEntity modifiedDocument = new NestedEntity();
            modifiedDocument.Foo = new SimpleEntity() { Foo = "baz" };

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.replace, operations.First().Operation);
            Assert.AreEqual("/Foo/Foo", operations.First().PropertyName);
            Assert.AreEqual(modifiedDocument.Foo.Foo, operations.First().Value);
        }

        [TestMethod]
        public void GenerateDiff_ArrayEntityAdd_ReturnsAddOperation()
        {
            ArrayEntity originalDocument = new ArrayEntity();
            originalDocument.Foo = new string[] { "bar" };
            ArrayEntity modifiedDocument = new ArrayEntity();
            modifiedDocument.Foo = new string[] { "bar", "baz" };

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.add, operations.First().Operation);
            Assert.AreEqual("/Foo/1", operations.First().PropertyName);
            Assert.AreEqual("baz", operations.First().Value);
        }

        [TestMethod]
        public void GenerateDiff_ArrayEntityRemove_ReturnsRemoveOperation()
        {
            ArrayEntity originalDocument = new ArrayEntity();
            originalDocument.Foo = new string[] { "bar" };
            ArrayEntity modifiedDocument = new ArrayEntity();
            modifiedDocument.Foo = new string[] {  };

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.remove, operations.First().Operation);
            Assert.AreEqual("/Foo/0", operations.First().PropertyName);
        }

        [TestMethod]
        public void GenerateDiff_ArrayEntityModify_ReturnsReplaceOperation()
        {
            ArrayEntity originalDocument = new ArrayEntity();
            originalDocument.Foo = new string[] { "bar" };
            ArrayEntity modifiedDocument = new ArrayEntity();
            modifiedDocument.Foo = new string[] { "baz" };

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.replace, operations.First().Operation);
            Assert.AreEqual("/Foo/0", operations.First().PropertyName);
            Assert.AreEqual("baz", operations.First().Value);
        }

        [TestMethod]
        public void GenerateDiff_ListEntityAdd_ReturnsAddOperation()
        {
            ListEntity originalDocument = new ListEntity();
            originalDocument.Foo = new List<string>() { "bar" };
            ListEntity modifiedDocument = new ListEntity();
            modifiedDocument.Foo = new List<string>() { "bar", "baz" };

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.add, operations.First().Operation);
            Assert.AreEqual("/Foo/1", operations.First().PropertyName);
            Assert.AreEqual("baz", operations.First().Value);
        }

        [TestMethod]
        public void GenerateDiff_ListEntityRemove_ReturnsRemoveOperation()
        {
            ListEntity originalDocument = new ListEntity();
            originalDocument.Foo = new List<string>() { "bar" };
            ListEntity modifiedDocument = new ListEntity();
            modifiedDocument.Foo = new List<string>();

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.remove, operations.First().Operation);
            Assert.AreEqual("/Foo/0", operations.First().PropertyName);
        }

        [TestMethod]
        public void GenerateDiff_ListEntityModify_ReturnsReplaceOperation()
        {
            ListEntity originalDocument = new ListEntity();
            originalDocument.Foo = new List<string>() { "bar" };
            ListEntity modifiedDocument = new ListEntity();
            modifiedDocument.Foo = new List<string>() { "baz" };

            List<JsonPatchOperation> operations = DiffHelper.GenerateDiff(originalDocument, modifiedDocument).ToList();

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(JsonPatchOperationType.replace, operations.First().Operation);
            Assert.AreEqual("/Foo/0", operations.First().PropertyName);
            Assert.AreEqual("baz", operations.First().Value);
        }
    }
}
