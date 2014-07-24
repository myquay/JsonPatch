using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPatch.Tests.Entitys;

namespace JsonPatch.Tests
{
    [TestClass]
    public class JsonPatchDocumentTests
    {

        #region JsonPatch Add Tests

        [TestMethod]
        public void Add_ValidPath_OperationAdded()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Add("Foo", "bar");

            //Assert
            Assert.AreEqual(1, patchDocument.Operations.Count);
            Assert.AreEqual(JsonPatchOperationType.add, patchDocument.Operations.Single().Operation);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void Add_InvalidPath_ThrowsJsonPatchParseException()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Add("FooMissing", "bar");
        }

        #endregion

        #region JsonPatch Remove Tests

        [TestMethod]
        public void Remove_ValidPath_OperationAdded()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Remove("Foo");

            //Assert
            Assert.AreEqual(1, patchDocument.Operations.Count);
            Assert.AreEqual(JsonPatchOperationType.remove, patchDocument.Operations.Single().Operation);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void Remove_InvalidPath_ThrowsJsonPatchParseException()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Remove("FooMissing");
        }

        #endregion

        #region JsonPatch Replace Tests

        [TestMethod]
        public void Replace_ValidPath_OperationAdded()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Replace("Foo", "bar");

            //Assert
            Assert.AreEqual(1, patchDocument.Operations.Count);
            Assert.AreEqual(JsonPatchOperationType.replace, patchDocument.Operations.Single().Operation);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void Replace_InvalidPath_ThrowsJsonPatchParseException()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Replace("FooMissing", "bar");
        }

        #endregion

        #region JsonPatch ApplyUpdatesTo Tests

        [TestMethod]
        public void ApplyUpdate_AddOperation_EntityUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();
            var entity = new SimpleEntity();

            //Act
            patchDocument.Add("Foo", "bar");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual("bar", entity.Foo);
        }

        [TestMethod]
        public void ApplyUpdate_RemoveOperation_EntityUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();
            var entity = new SimpleEntity { Foo = "bar" };

            //Act
            patchDocument.Remove("Foo");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual(null, entity.Foo);
        }

        [TestMethod]
        public void ApplyUpdate_ReplaceOperation_EntityUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();
            var entity = new SimpleEntity { Foo = "bar" };

            //Act
            patchDocument.Replace("Foo", "baz");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual("baz", entity.Foo);
        } 

        #endregion

    }
}
