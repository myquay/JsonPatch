using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPatch.Helpers;
using JsonPatch.Tests.Entitys;
using System.Collections.Generic;

namespace JsonPatch.Tests
{
    [TestClass]
    public class PathHelperTests
    {
        //IsPathValid

        #region Invalid Path Names

        [TestMethod]
        public void IsPathValid_BlankPath_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), "");

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_BlankNullPath_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), null);

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_PathBeginningWithANumber_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), "/8/Foo");

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_PathWithEmptyComponent_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), "//");

            //assert
            Assert.IsTrue(isValid == false);
        }

        #endregion

        #region Missing Path Names

        [TestMethod]
        public void IsPathValid_SimpleMissingPath_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), "/MissingFoo");

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_NonIndexOnArray_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ArrayEntity), "/Foo/NotAnIndex");

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_MissingPathOnChildEntity_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/Bax/1/MissingFoo");

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_ChildIndexerOnNonArray_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/Baz/1/1");

            //assert
            Assert.IsTrue(isValid == false);
        }

        #endregion

        #region Valid paths

        [TestMethod]
        public void IsPathValid_SimplePath_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), "/Foo");

            //assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsPathValid_ArrayPath_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ArrayEntity), "/Foo/3");

            //assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsPathValid_PathOnArrayEntity_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ArrayEntity), "/Foo/3");

            //assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsPathValid_ChildPathOnArray_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/Baz/1/Foo");

            //assert
            Assert.IsTrue(isValid);
        }

		[TestMethod]
		public void IsPathValid_ChildPathOnList_ReturnsTrue()
		{
			//act
			var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/Qux/1/Foo");

			//assert
			Assert.IsTrue(isValid);
		}

        [TestMethod]
        public void IsPathValid_PathOnChildArray_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/Foo/Foo/1");

            //assert
            Assert.IsTrue(isValid);
        }

        #endregion

        //SetValueFromPath

        #region Operations on simple paths

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_InvalidPath_ThrowsJsonPatchException()
        {
            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "",  new SimpleEntity { }, null, JsonPatchOperationType.add);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathAddValueToNull_UpdatesValue()
        {
            //arrange
            var entity = new SimpleEntity { };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/Foo", entity, "New Value", JsonPatchOperationType.add);

            //assert
            Assert.AreEqual("New Value", entity.Foo);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_SimplePathAddValueToNonNull_ThrowsJsonPatchException()
        {
            //arrange
            var entity = new SimpleEntity { Foo = "Existing Value" };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/Foo", entity, "New Value", JsonPatchOperationType.add);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathReplaceValueFromNull_UpdatesValue()
        {
            //arrange
            var entity = new SimpleEntity { };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/Foo", entity, "New Value", JsonPatchOperationType.replace);

            //assert
            Assert.AreEqual("New Value", entity.Foo);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathReplaceValueFromNonNull_UpdatesValue()
        {
            //arrange
            var entity = new SimpleEntity { Foo = "Existing Value" };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/Foo", entity, "New Value", JsonPatchOperationType.replace);
            
            //assert
            Assert.AreEqual("New Value", entity.Foo);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathRemoveValueFromNull_ValueIsNull()
        {
            //arrange
            var entity = new SimpleEntity { };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/Foo", entity, null, JsonPatchOperationType.remove);

            //assert
            Assert.AreEqual(null, entity.Foo);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathReplaceValueFromNonNull_ValueIsNull()
        {
            //arrange
            var entity = new SimpleEntity { Foo = "Existing Value" };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/Foo", entity, null, JsonPatchOperationType.remove);

            //assert
            Assert.AreEqual(null, entity.Foo);
        }

        #endregion

        #region operations on array indexes

        [TestMethod]
        public void SetValueFromPath_ReplaceArrayValue_UpdatesValue()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/Foo/1", entity, "Element Two Updated", JsonPatchOperationType.replace);

            //Assert
            Assert.AreEqual("Element Two Updated", entity.Foo[1]);
            Assert.AreEqual(2, entity.Foo.Length);
        }

		[TestMethod]
		public void SetValueFromPath_ReplaceListValue_UpdatesValue()
		{
			//Arrange
			var entity = new ListEntity
			{
				Foo = new List<string> { "Element One", "Element Two" }
			};

			//act
			PathHelper.SetValueFromPath(typeof(ListEntity), "/Foo/1", entity, "Element Two Updated", JsonPatchOperationType.replace);

			//Assert
			Assert.AreEqual("Element Two Updated", entity.Foo[1]);
			Assert.AreEqual(2, entity.Foo.Count);
		}

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_ReplaceIndexOutOfBounds_ThrowsJsonPatchException()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/Foo/2", entity, "Element Two Updated", JsonPatchOperationType.replace);
        }

        [TestMethod]
        public void SetValueFromPath_AddArrayValue_AddsValue()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/Foo/1", entity, "Element Two Updated", JsonPatchOperationType.add);

            //Assert
            Assert.AreEqual("Element Two Updated", entity.Foo[1]);
            Assert.AreEqual("Element Two", entity.Foo[2]);
            Assert.AreEqual(3, entity.Foo.Length);
        }

		[TestMethod]
		public void SetValueFromPath_AddListValue_AddsValue()
		{
			//Arrange
			var entity = new ListEntity
			{
				Foo = new List<string> { "Element One", "Element Two" }
			};

			//act
			PathHelper.SetValueFromPath(typeof(ListEntity), "/Foo/1", entity, "Element Two Updated", JsonPatchOperationType.add);

			//Assert
			Assert.AreEqual("Element Two Updated", entity.Foo[1]);
			Assert.AreEqual("Element Two", entity.Foo[2]);
			Assert.AreEqual(3, entity.Foo.Count);
		}

        [TestMethod]
        public void SetValueFromPath_AddArrayValueAtEnd_AddsValue()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/Foo/2", entity, "Element Two Updated", JsonPatchOperationType.add);

            //Assert
            Assert.AreEqual("Element Two Updated", entity.Foo[2]);
            Assert.AreEqual("Element Two", entity.Foo[1]);
            Assert.AreEqual(3, entity.Foo.Length);
        }

		[TestMethod]
		public void SetValueFromPath_AddListValueAtEnd_AddsValue()
		{
			//Arrange
			var entity = new ListEntity
			{
				Foo = new List<string> { "Element One", "Element Two" }
			};

			//act
			PathHelper.SetValueFromPath(typeof(ListEntity), "/Foo/2", entity, "Element Two Updated", JsonPatchOperationType.add);

			//Assert
			Assert.AreEqual("Element Two Updated", entity.Foo[2]);
			Assert.AreEqual("Element Two", entity.Foo[1]);
			Assert.AreEqual(3, entity.Foo.Count);
		}

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_AddIndexOutOfBounds_ThrowsJsonPatchException()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/Foo/3", entity, "Element Two Updated", JsonPatchOperationType.add);
        }

        [TestMethod]
        public void SetValueFromPath_RemoveArrayValueFromStart_RemovesValue()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/Foo/0", entity, null, JsonPatchOperationType.remove);

            //Assert
            Assert.AreEqual("Element Two", entity.Foo[0]);
            Assert.AreEqual(1, entity.Foo.Length);
        }

        [TestMethod]
        public void SetValueFromPath_RemoveArrayValueFromEnd_RemovesValue()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/Foo/1", entity, null, JsonPatchOperationType.remove);

            //Assert
            Assert.AreEqual("Element One", entity.Foo[0]);
            Assert.AreEqual(1, entity.Foo.Length);
        }

		[TestMethod]
		public void SetValueFromPath_RemoveListValueFromStart_RemovesValue()
		{
			//Arrange
			var entity = new ListEntity
			{
				Foo = new List<string> { "Element One", "Element Two" }
			};

			//act
			PathHelper.SetValueFromPath(typeof(ListEntity), "/Foo/0", entity, null, JsonPatchOperationType.remove);

			//Assert
			Assert.AreEqual("Element Two", entity.Foo[0]);
			Assert.AreEqual(1, entity.Foo.Count);
		}

		[TestMethod]
		public void SetValueFromPath_RemoveListValueFromEnd_RemovesValue()
		{
			//Arrange
			var entity = new ListEntity
			{
				Foo = new List<string> { "Element One", "Element Two" }
			};

			//act
			PathHelper.SetValueFromPath(typeof(ListEntity), "/Foo/1", entity, null, JsonPatchOperationType.remove);

			//Assert
			Assert.AreEqual("Element One", entity.Foo[0]);
			Assert.AreEqual(1, entity.Foo.Count);
		}


        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_RemoveIndexOutOfBounds_ThrowsJsonPatchException()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/Foo/2", entity, null, JsonPatchOperationType.remove);
        }

        #endregion

        #region Operations on complex paths

        [TestMethod]
        public void SetValueFromPath_SetArrayAddValueToNull_AddsValue()
        {
            //arrange
            var entity = new ComplexEntity { };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/Foo/Foo", entity, new[]{"Element One", "Element Two"}, JsonPatchOperationType.add);

            //assert
            Assert.IsNotNull(entity.Foo);
            Assert.IsNotNull(entity.Foo.Foo);
            Assert.AreEqual(2, entity.Foo.Foo.Length);
        }

        [TestMethod]
        public void SetValueFromPath_AddToEmptyArray_CreatesArrayAndAddsValue()
        {
            //arrange
            var entity = new ComplexEntity { };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/Foo/Foo/0", entity, "Element One", JsonPatchOperationType.add);

            //assert
            Assert.IsNotNull(entity.Foo);
            Assert.IsNotNull(entity.Foo.Foo);
            Assert.AreEqual(1, entity.Foo.Foo.Length);
            Assert.AreEqual("Element One", entity.Foo.Foo[0]);
        }

        [TestMethod]
        public void SetValueFromPath_NestedAddToEmptyArray_CreatesArrayAndAddsValue()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Baz = new SimpleEntity[] { new SimpleEntity{ Foo = "Foo" } }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/Baz/0/Foo", entity, "New Value", JsonPatchOperationType.replace);

            //assert
            Assert.AreEqual("New Value", entity.Baz[0].Foo);
        }

		[TestMethod]
		public void SetValueFromPath_NestedAddToEmptyList_CreatesListAndAddsValue()
		{
			//arrange
			var entity = new ComplexEntity
			{
				Qux = new List<SimpleEntity> { new SimpleEntity { Foo = "Foo" } }
			};

			//act
			PathHelper.SetValueFromPath(typeof(ComplexEntity), "/Qux/0/Foo", entity, "New Value", JsonPatchOperationType.replace);

			//assert
			Assert.AreEqual("New Value", entity.Qux[0].Foo);
		}
        #endregion
    }
}
