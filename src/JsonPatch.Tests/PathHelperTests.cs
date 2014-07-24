using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPatch.Helpers;
using JsonPatch.Tests.Entitys;

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
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "", new SimpleEntity { }, null, JsonPatchOperationType.add);
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

        #endregion
    }
}
