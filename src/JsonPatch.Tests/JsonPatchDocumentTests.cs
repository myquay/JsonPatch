using JsonPatch.Tests.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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

        #region JsonPatch Move Tests

        [TestMethod]
        public void Move_ValidPaths_OperationAdded()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Move("Foo", "Baz");

            //Assert
            Assert.AreEqual(1, patchDocument.Operations.Count);
            Assert.AreEqual(JsonPatchOperationType.move, patchDocument.Operations.Single().Operation);
        }

        [TestMethod]
        public void Move_ArrayIndexes_OperationAdded()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<ArrayEntity>();

            //Act
            patchDocument.Move("Foo/5", "Foo/2");

            //Assert
            Assert.AreEqual(1, patchDocument.Operations.Count);
            Assert.AreEqual(JsonPatchOperationType.move, patchDocument.Operations.Single().Operation);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void Move_InvalidFromPath_ThrowsJsonPatchParseException()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Move("FooMissing", "Baz");
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void Move_InvalidDestinationPath_ThrowsJsonPatchParseException()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Move("Foo", "BazMissing");
        }

        #endregion

        #region JsonPatch Test Tests

        [TestMethod]
        public void Test_ValidPath_OperationAdded()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Test("Foo", "bar");

            //Assert
            Assert.AreEqual(1, patchDocument.Operations.Count);
            Assert.AreEqual(JsonPatchOperationType.test, patchDocument.Operations.Single().Operation);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void Test_InvalidPath_ThrowsJsonPatchParseException()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Test("FooMissing", "bar");
        }

        #endregion

        #region JsonPatch ApplyUpdatesTo Tests

        #region Add Operation

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

        #endregion

        #region Remove Operation

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

        #endregion

        #region Replace Operation

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

        [TestMethod]
        public void ApplyUpdate_ReplaceOperation_BadEnum()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<EnumEntity>();
            var entity = new EnumEntity();
            var badEnumValue = "Garbage";

            //Act
            patchDocument.Replace(nameof(EnumEntity.Foo), badEnumValue);
            var myException = Assert.ThrowsException<JsonPatchOperationException>(() => patchDocument.ApplyUpdatesTo(entity));

            //Assert
            Assert.AreEqual(myException.Message, "Failed to set value at path \"/Foo\" while performing \"replace\" operation: Requested value 'Garbage' was not found.");
            Assert.AreEqual(myException.operationType, JsonPatchOperationType.replace);
            Assert.AreEqual(myException.path, $"/{nameof(EnumEntity.Foo)}");
            Assert.AreEqual(myException.entityType, entity.Foo.GetType());
            Assert.AreEqual(myException.value, badEnumValue);
        }

        [TestMethod]
        public void ApplyUpdate_ReplaceOperation_BadNullableEnum()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<EnumNullableEntity>();
            var entity = new EnumNullableEntity();
            var badEnumValue = "Garbage";

            //Act
            patchDocument.Replace(nameof(EnumNullableEntity.Foo), badEnumValue);
            var myException = Assert.ThrowsException<JsonPatchOperationException>(() => patchDocument.ApplyUpdatesTo(entity));

            //Assert
            Assert.AreEqual("Failed to set value at path \"/Foo\" while performing \"replace\" operation: Requested value 'Garbage' was not found.", myException.Message);
            Assert.AreEqual(myException.operationType, JsonPatchOperationType.replace);
            Assert.AreEqual(myException.path, $"/{nameof(EnumNullableEntity.Foo)}");
            Assert.AreEqual(myException.entityType, typeof(EnumNullableEntity).GetProperty(nameof(EnumNullableEntity.Foo)).PropertyType);
            Assert.AreEqual(myException.value, badEnumValue);
        }

        [TestMethod]
        public void ApplyUpdate_ReplaceOperation_GoodNullableEnum()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<EnumNullableEntity>();
            var entity = new EnumNullableEntity();
            var goodEnumValue = "FirstEnum";

            //Act
            patchDocument.Replace(nameof(EnumNullableEntity.Foo), goodEnumValue);
            patchDocument.ApplyUpdatesTo(entity);


            //Assert
            Assert.AreEqual(SampleEnum.FirstEnum, entity.Foo);
        }

        [TestMethod]
        public void ApplyUpdate_ReplaceOperation_LowercaseEnum()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<EnumEntity>();
            var entity = new EnumEntity
            {
                Foo = SampleEnum.FirstEnum
            };

            //Act
            patchDocument.Replace(nameof(EnumEntity.Foo), "secondenum");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual(entity.Foo, SampleEnum.SecondEnum);
        }

        [TestMethod]
        public void ApplyUpdate_ReplaceOperation_LowercaseNullableEnum()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<EnumNullableEntity>();
            var entity = new EnumNullableEntity
            {
                Foo = SampleEnum.FirstEnum
            };

            //Act
            patchDocument.Replace(nameof(EnumNullableEntity.Foo), "secondenum");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual(entity.Foo, SampleEnum.SecondEnum);
        }

        [TestMethod]
        public void ApplyUpdate_ReplaceOperation_LowercaseNullableEnum_Null()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<EnumNullableEntity>();
            var entity = new EnumNullableEntity
            {
                Foo = null
            };

            //Act
            patchDocument.Replace(nameof(EnumNullableEntity.Foo), "secondenum");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual(entity.Foo, SampleEnum.SecondEnum);
        }
        #endregion

        #region Move Operation

        [TestMethod]
        public void ApplyUpdate_MoveOperation_EntityUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();
            var entity = new SimpleEntity { Foo = "bar", Baz = "qux" };

            //Act
            patchDocument.Move("Foo", "Baz");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.IsNull(entity.Foo);
            Assert.AreEqual("bar", entity.Baz);
        }

        [TestMethod]
        public void ApplyUpdate_MoveListElement_EntityUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<ListEntity>();
            var entity = new ListEntity
            {
                Foo = new List<string> { "Element One", "Element Two", "Element Three" }
            };

            //Act
            patchDocument.Move("/Foo/2", "/Foo/1");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual(3, entity.Foo.Count);
            Assert.AreEqual("Element One", entity.Foo[0]);
            Assert.AreEqual("Element Three", entity.Foo[1]);
            Assert.AreEqual("Element Two", entity.Foo[2]);
        }

        [TestMethod]
        public void ApplyUpdate_MoveFromListToProperty_EntityUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<ComplexEntity>();
            var entity = new ComplexEntity
            {
                Bar = new SimpleEntity(),
                Norf = new List<ListEntity>
                {
                    new ListEntity
                    {
                        Foo = new List<string> { "Element One", "Element Two", "Element Three" }
                    }
                }
            };

            //Act
            patchDocument.Move("/Norf/0/Foo/1", "/Bar/Foo");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual(2, entity.Norf[0].Foo.Count);
            Assert.AreEqual("Element One", entity.Norf[0].Foo[0]);
            Assert.AreEqual("Element Three", entity.Norf[0].Foo[1]);
            Assert.AreEqual("Element Two", entity.Bar.Foo);
        }

        [TestMethod]
        public void ApplyUpdate_MoveFromPropertyToList_EntityUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<ComplexEntity>();
            var entity = new ComplexEntity
            {
                Bar = new SimpleEntity
                {
                    Foo = "I am foo"
                },
                Norf = new List<ListEntity>
                {
                    new ListEntity
                    {
                        Foo = new List<string> { "Element One", "Element Two", "Element Three" }
                    }
                }
            };

            //Act
            patchDocument.Move("/Bar/Foo", "/Norf/0/Foo/1");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.IsNull(entity.Bar.Foo);
            Assert.AreEqual(4, entity.Norf[0].Foo.Count);
            Assert.AreEqual("Element One", entity.Norf[0].Foo[0]);
            Assert.AreEqual("I am foo", entity.Norf[0].Foo[1]);
            Assert.AreEqual("Element Two", entity.Norf[0].Foo[2]);
            Assert.AreEqual("Element Three", entity.Norf[0].Foo[3]);
        }

        #endregion

        #region Test Operation

        [TestMethod]
        public void ApplyUpdate_TestOperation_ConditionPassed_EntityUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();
            var entity = new SimpleEntity { Foo = "bar" };

            //Act
            patchDocument.Test("Foo", "bar");
            patchDocument.Replace("Foo", "baz");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreEqual("baz", entity.Foo);
        }

        [TestMethod]
        public void ApplyUpdate_TestOperation_ConditionFailed_EntityNotUpdated()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();
            var entity = new SimpleEntity { Foo = "not bar" };

            //Act
            patchDocument.Test("Foo", "bar");
            patchDocument.Replace("Foo", "baz");
            patchDocument.ApplyUpdatesTo(entity);

            //Assert
            Assert.AreNotEqual("baz", entity.Foo);
        }

        #endregion

        #endregion

        #region JsonPatch HasOperations Tests

        [TestMethod]
        public void HasOperations_FalseByDefault()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Assert
            Assert.IsFalse(patchDocument.HasOperations);
        }

        [TestMethod]
        public void Add_ValidPath_SetsHasOperations()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Add("Foo", "bar");

            //Assert
            Assert.IsTrue(patchDocument.HasOperations);
        }

        public void Remove_ValidPath_SetsHasOperations()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Remove("Foo");

            //Assert
            Assert.IsTrue(patchDocument.HasOperations);
        }

        [TestMethod]
        public void Replace_ValidPath_SetsHasOperations()
        {
            //Arrange
            var patchDocument = new JsonPatchDocument<SimpleEntity>();

            //Act
            patchDocument.Replace("Foo", "bar");

            //Assert
            Assert.IsTrue(patchDocument.HasOperations);
        }
        #endregion

    }
}
