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
        public void Add_ValidPath_OperationSuccessfullyAdded()
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
        public void Remove_ValidPath_OperationSuccessfullyAdded()
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
        public void Replace_ValidPath_OperationSuccessfullyAdded()
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
        
    }
}
