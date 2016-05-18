using System;
using JsonPatch.Paths;
using JsonPatch.Paths.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPatch.Tests.Entitys;
using System.Collections.Generic;
using JsonPatch.Formatting;
using JsonPatch.Paths.Resolvers;

namespace JsonPatch.Tests
{
    [TestClass]
    public class CaseInsensitivePathHelperTests
    {
        private CaseInsensitivePropertyPathResolver _resolver;

        [TestInitialize]
        public void SetResolver()
        {
            _resolver = new CaseInsensitivePropertyPathResolver();
            var settings = new JsonPatchSettings
            {
                PathResolver = _resolver
            };

            var formatter = new JsonPatchFormatter(settings);
        }

        #region ParsePath

        [TestMethod]
        public void ParsePath_SimpleProperty_ParsesSuccessfully()
        {
            //act
            var pathComponents = PathHelper.ParsePath("bar", typeof(SimpleEntity));

            //assert
            Assert.AreEqual(1, pathComponents.Length);
            Assert.AreEqual("Bar", pathComponents[0].Name);
            Assert.IsInstanceOfType(pathComponents[0], typeof(PropertyPathComponent));
            Assert.AreEqual(typeof(int), pathComponents[0].ComponentType);
            Assert.IsFalse(pathComponents[0].IsCollection);
        }

        [TestMethod]
        public void ParsePath_LeadingSlash_SlashIgnored()
        {
            //act
            var pathComponents = PathHelper.ParsePath("/foo", typeof(SimpleEntity));

            //assert
            Assert.AreEqual(1, pathComponents.Length);
            Assert.AreEqual("Foo", pathComponents[0].Name);
        }

        [TestMethod]
        public void ParsePath_LeadingAndTrailingSlashes_SlashesIgnored()
        {
            //act
            var pathComponents = PathHelper.ParsePath("/foo/", typeof(SimpleEntity));

            //assert
            Assert.AreEqual(1, pathComponents.Length);
            Assert.AreEqual("Foo", pathComponents[0].Name);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void ParsePath_InvalidProperty_ThrowsException()
        {
            //act
            PathHelper.ParsePath("quux", typeof(SimpleEntity));
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void ParsePath_EmptyPath_ThrowsException()
        {
            //act
            PathHelper.ParsePath("", typeof(SimpleEntity));
        }

        [TestMethod]
        public void ParsePath_CollectionIndex_ParsesSuccessfully()
        {
            //act
            var pathComponents = PathHelper.ParsePath("/foo/5", typeof(ListEntity));

            //assert
            Assert.AreEqual(2, pathComponents.Length);
            Assert.AreEqual("Foo", pathComponents[0].Name);
            Assert.IsTrue(pathComponents[0].IsCollection);
            Assert.AreEqual("5", pathComponents[1].Name);
            Assert.IsInstanceOfType(pathComponents[1], typeof(CollectionIndexPathComponent));
            Assert.AreEqual(5, ((CollectionIndexPathComponent) pathComponents[1]).CollectionIndex);
            Assert.AreEqual(typeof(string), pathComponents[1].ComponentType);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void ParsePath_CollectionIndexAfterNonCollectionProperty_ParsesSuccessfully()
        {
            //act
            PathHelper.ParsePath("/bar/5", typeof(SimpleEntity));
        }

        #endregion

        #region IsPathValid

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
        public void IsPathValid_NullPath_ReturnsFalse()
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
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), "/8/foo");

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
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), "/missingFoo");

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_NonIndexOnArray_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ArrayEntity), "/foo/NotAnIndex");

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_MissingPathOnChildEntity_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/bax/1/MissingFoo");

            //assert
            Assert.IsTrue(isValid == false);
        }

        [TestMethod]
        public void IsPathValid_ChildIndexerOnNonArray_ReturnsFalse()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/baz/1/1");

            //assert
            Assert.IsTrue(isValid == false);
        }

        #endregion

        #region Valid paths

        [TestMethod]
        public void IsPathValid_SimplePath_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(SimpleEntity), "/foo");

            //assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsPathValid_ArrayPath_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ArrayEntity), "/foo/3");

            //assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsPathValid_PathOnArrayEntity_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ArrayEntity), "/foo/3");

            //assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsPathValid_ChildPathOnArray_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/baz/1/foo");

            //assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsPathValid_ChildPathOnList_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/quX/1/foo");

            //assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsPathValid_PathOnChildArray_ReturnsTrue()
        {
            //act
            var isValid = PathHelper.IsPathValid(typeof(ComplexEntity), "/foo/foo/1");

            //assert
            Assert.IsTrue(isValid);
        }

        #endregion

        #endregion

        #region GetValueFromPath

        #region Operations on simple paths

        [TestMethod]
        public void GetValueFromPath_SimplePropertyOnRoot_ReturnsValue()
        {
            //arrange
            var entity = new SimpleEntity
            {
                Foo = "I am foo"
            };

            //act
            var value = PathHelper.GetValueFromPath(typeof(SimpleEntity), "/foo", entity);

            //assert
            Assert.AreEqual("I am foo", value);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void GetValueFromPath_NullRoot_ThrowsException()
        {
            //act
            PathHelper.GetValueFromPath(typeof(SimpleEntity), "/foo", null);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void GetValueFromPath_MissingProperty_ThrowsException()
        {
            //arrange
            var entity = new SimpleEntity
            {
                Foo = "I am foo"
            };

            //act
            PathHelper.GetValueFromPath(typeof(SimpleEntity), "/Foomissing", entity);
        }

        #endregion

        #region operations on list/array indexes

        [TestMethod]
        public void GetValueFromPath_CollectionIndexOnRoot_ReturnsValue()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            var value = PathHelper.GetValueFromPath(typeof(ArrayEntity), "/foo/1", entity);

            //Assert
            Assert.AreEqual("Element Two", value);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void GetValueFromPath_InvalidCollectionIndex_ThrowsException()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.GetValueFromPath(typeof(ArrayEntity), "/foo/5", entity);
        }

        #endregion

        #region Operations on complex paths

        [TestMethod]
        public void GetValueFromPath_ComplexPath_ReturnsValue()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Bar = new SimpleEntity
                {
                    Foo = "I am foo"
                }
            };

            //act
            var value = PathHelper.GetValueFromPath(typeof(ComplexEntity), "/bar/foo", entity);

            //assert
            Assert.AreEqual("I am foo", value);
        }

        [TestMethod]
        public void GetValueFromPath_ValidCollectionIndexOnComplexObject_ReturnsValue()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Norf = new List<ListEntity>
                {
                    new ListEntity { Foo = new List<string> { "A1", "A2", "A3" } },
                    new ListEntity { Foo = new List<string> { "B1", "B2", "B3" } }
                }
            };

            //act
            var value = PathHelper.GetValueFromPath(typeof(ComplexEntity), "/norf/1/foo/2", entity);

            //assert
            Assert.AreEqual("B3", value);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void GetValueFromPath_NullParent_ThrowsException()
        {
            //arrange
            var entity = new ComplexEntity { };

            //act
            PathHelper.GetValueFromPath(typeof(ComplexEntity), "/bar/foo", entity);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void GetValueFromPath_InvalidCollectionIndexOnIntermediatePathComponent_ThrowsException()
        {
            //Arrange
            var entity = new ComplexEntity
            {
                Qux = new List<SimpleEntity>
                {
                    new SimpleEntity(),
                    new SimpleEntity { Foo = "I am foo" }
                }
            };

            //act
            PathHelper.GetValueFromPath(typeof(ComplexEntity), "/qux/5/foo", entity);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void GetValueFromPath_NullCollection_ThrowsException()
        {
            //Arrange
            var entity = new ComplexEntity
            {
                Norf = new List<ListEntity> { null }
            };

            //act
            PathHelper.GetValueFromPath(typeof(ComplexEntity), "/norf/0/foo/0", entity);
        }

        #endregion

        #endregion

        #region SetValueFromPath

        #region Operations on simple paths

        [TestMethod, ExpectedException(typeof(JsonPatchParseException))]
        public void SetValueFromPath_InvalidPath_ThrowsException()
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
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/foo", entity, "New Value", JsonPatchOperationType.add);

            //assert
            Assert.AreEqual("New Value", entity.Foo);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathAddValueToNonNull_UpdatesValue()
        {
            //arrange
            var entity = new SimpleEntity { Foo = "Existing Value" };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/foo", entity, "New Value", JsonPatchOperationType.add);

            //assert
            Assert.AreEqual("New Value", entity.Foo);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathReplaceValueFromNull_UpdatesValue()
        {
            //arrange
            var entity = new SimpleEntity { };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/foo", entity, "New Value", JsonPatchOperationType.replace);

            //assert
            Assert.AreEqual("New Value", entity.Foo);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathReplaceValueFromNonNull_UpdatesValue()
        {
            //arrange
            var entity = new SimpleEntity { Foo = "Existing Value" };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/foo", entity, "New Value", JsonPatchOperationType.replace);
            
            //assert
            Assert.AreEqual("New Value", entity.Foo);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathRemoveValueFromNull_ValueIsNull()
        {
            //arrange
            var entity = new SimpleEntity { };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/foo", entity, null, JsonPatchOperationType.remove);

            //assert
            Assert.AreEqual(null, entity.Foo);
        }

        [TestMethod]
        public void SetValueFromPath_SimplePathReplaceValueFromNonNull_ValueIsNull()
        {
            //arrange
            var entity = new SimpleEntity { Foo = "Existing Value" };

            //act
            PathHelper.SetValueFromPath(typeof(SimpleEntity), "/foo", entity, null, JsonPatchOperationType.remove);

            //assert
            Assert.AreEqual(null, entity.Foo);
        }

        #endregion

        #region operations on list/array indexes

        #region Array tests

        [TestMethod]
        public void SetValueFromPath_ReplaceArrayValue_UpdatesValue()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/foo/1", entity, "Element Two Updated", JsonPatchOperationType.replace);

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
            PathHelper.SetValueFromPath(typeof(ListEntity), "/foo/1", entity, "Element Two Updated", JsonPatchOperationType.replace);

            //Assert
            Assert.AreEqual("Element Two Updated", entity.Foo[1]);
            Assert.AreEqual(2, entity.Foo.Count);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_ReplaceIndexOutOfBounds_ThrowsException()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/foo/2", entity, "Element Two Updated", JsonPatchOperationType.replace);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_AddArrayValue_ThrowsError()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/foo/2", entity, "Element Three", JsonPatchOperationType.add);

            // Arrays should not support resizing. Expect JsonPatchException with an inner exception of type
            // NotSupportedException: Collection was of a fixed size.
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_RemoveArrayValue_ThrowsException()
        {
            //Arrange
            var entity = new ArrayEntity
            {
                Foo = new string[] { "Element One", "Element Two", "Element Three" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ArrayEntity), "/foo/1", entity, null, JsonPatchOperationType.remove);

            // Arrays should not support resizing. Expect JsonPatchException with an inner exception of type
            // NotSupportedException: Collection was of a fixed size
        }

        #endregion

        #region List tests

        [TestMethod]
        public void SetValueFromPath_AddListValue_AddsValue()
        {
            //Arrange
            var entity = new ListEntity
            {
                Foo = new List<string> { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ListEntity), "/foo/1", entity, "Element Two Updated", JsonPatchOperationType.add);

            //Assert
            Assert.AreEqual("Element Two Updated", entity.Foo[1]);
            Assert.AreEqual("Element Two", entity.Foo[2]);
            Assert.AreEqual(3, entity.Foo.Count);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_AddToNullList_ThrowsException()
        {
            //Arrange
            var entity = new ListEntity();

            //act
            PathHelper.SetValueFromPath(typeof(ListEntity), "/foo/0", entity, "Element One", JsonPatchOperationType.add);
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
            PathHelper.SetValueFromPath(typeof(ListEntity), "/foo/2", entity, "Element Two Updated", JsonPatchOperationType.add);

            //Assert
            Assert.AreEqual("Element Two Updated", entity.Foo[2]);
            Assert.AreEqual("Element Two", entity.Foo[1]);
            Assert.AreEqual(3, entity.Foo.Count);
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
            PathHelper.SetValueFromPath(typeof(ListEntity), "/foo/0", entity, null, JsonPatchOperationType.remove);

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
            PathHelper.SetValueFromPath(typeof(ListEntity), "/foo/1", entity, null, JsonPatchOperationType.remove);

            //Assert
            Assert.AreEqual("Element One", entity.Foo[0]);
            Assert.AreEqual(1, entity.Foo.Count);
        }


        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_RemoveIndexOutOfBounds_ThrowsException()
        {
            //Arrange
            var entity = new ListEntity
            {
                Foo = new List<string> { "Element One", "Element Two" }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ListEntity), "/foo/2", entity, null, JsonPatchOperationType.remove);
        }

        #endregion
        
        #region Dictionary tests

        [TestMethod]
        public void SetValueFromPath_ReplaceDictionaryByStringKey_UpdatesValue()
        {
            //Arrange
            var entity = new DictionaryEntity<string>
            {
                Foo = new Dictionary<string, string> { { "key1", "Element One" }, { "key2", "Element Two" }, }
            };

            //act
            PathHelper.SetValueFromPath(typeof(DictionaryEntity<string>), "/foo/key1", entity, "Element One Updated", JsonPatchOperationType.replace);

            //Assert
            Assert.AreEqual("Element One Updated", entity.Foo["key1"]);
            Assert.AreEqual("Element Two", entity.Foo["key2"]);
            Assert.AreEqual(2, entity.Foo.Count);
        }

        [TestMethod]
        public void SetValueFromPath_ReplaceDictionaryByIntKey_UpdatesValue()
        {
            //Arrange
            var entity = new DictionaryEntity<int>
            {
                Foo = new Dictionary<int, string> { { 1, "Element One" }, { 2, "Element Two" }, }
            };

            //act
            PathHelper.SetValueFromPath(typeof(DictionaryEntity<int>), "/foo/2", entity, "Element Two Updated", JsonPatchOperationType.replace);

            //Assert
            Assert.AreEqual("Element Two Updated", entity.Foo[2]);
            Assert.AreEqual("Element One", entity.Foo[1]);
            Assert.AreEqual(2, entity.Foo.Count);
        }

        [TestMethod]
        public void SetValueFromPath_AddDictionaryValue_AddsValue()
        {
            //Arrange
            var entity = new DictionaryEntity<int>
            {
                Foo = new Dictionary<int, string> { { 1, "Element One" }, { 2, "Element Two" }, }
            };

            //act
            PathHelper.SetValueFromPath(typeof(DictionaryEntity<int>), "/foo/3", entity, "Element Three", JsonPatchOperationType.add);

            //Assert
            Assert.AreEqual("Element Three", entity.Foo[3]);
            Assert.AreEqual(3, entity.Foo.Count);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_AddDictionaryValue_ThrowsForExistingKey()
        {
            //Arrange
            var entity = new DictionaryEntity<int>
            {
                Foo = new Dictionary<int, string> { { 1, "Element One" }, { 2, "Element Two" }, }
            };

            //act
            PathHelper.SetValueFromPath(typeof(DictionaryEntity<int>), "/foo/2", entity, "Element Three", JsonPatchOperationType.add);
        }

        [TestMethod]
        public void SetValueFromPath_RemoveDictionaryValue_RemovesValue()
        {
            //Arrange
            var entity = new DictionaryEntity<int>
            {
                Foo = new Dictionary<int, string> { { 1, "Element One" }, { 2, "Element Two" }, }
            };

            //act
            PathHelper.SetValueFromPath(typeof(DictionaryEntity<int>), "/foo/2", entity, null, JsonPatchOperationType.remove);

            //Assert
            Assert.IsFalse(entity.Foo.ContainsKey(2));
            Assert.AreEqual(1, entity.Foo.Count);
        }

        [TestMethod]
        public void SetValueFromPath_RemoveDictionaryValue_NoOpForNonexistingKey()
        {
            //Arrange
            var entity = new DictionaryEntity<int>
            {
                Foo = new Dictionary<int, string> { { 1, "Element One" }, { 2, "Element Two" }, }
            };

            //act
            PathHelper.SetValueFromPath(typeof(DictionaryEntity<int>), "/foo/5", entity, null, JsonPatchOperationType.remove);

            //Assert
            Assert.AreEqual(2, entity.Foo.Count);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_AddToNullDictionary_ThrowsException()
        {
            //Arrange
            var entity = new DictionaryEntity<int>();

            //act
            PathHelper.SetValueFromPath(typeof(DictionaryEntity<int>), "/foo/0", entity, "Element One", JsonPatchOperationType.add);
        }

        #endregion

        #endregion

        #region Operations on complex paths

        [TestMethod]
        public void SetValueFromPath_ValidParent_SetsValue()
        {
            //arrange
            var entity = new ComplexEntity { Bar = new SimpleEntity() };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/bar/foo", entity, "New Value", JsonPatchOperationType.add);

            //assert
            Assert.AreEqual("New Value", entity.Bar.Foo);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_NullParent_ThrowsException()
        {
            //arrange
            var entity = new ComplexEntity { };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/bar/foo", entity, "New Value", JsonPatchOperationType.add);
        }

        [TestMethod]
        public void SetValueFromPath_AddToListItem_SetsValue()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Qux = new List<SimpleEntity>
                {
                    new SimpleEntity()
                }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/qux/0/foo", entity, "New Value", JsonPatchOperationType.add);

            //assert
            Assert.AreEqual("New Value", entity.Qux[0].Foo);
        }

        [TestMethod]
        public void SetValueFromPath_ReplaceInListItem_SetsValue()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Qux = new List<SimpleEntity>
                {
                    new SimpleEntity { Foo = "Old Value" }
                }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/Qux/0/foo", entity, "New Value", JsonPatchOperationType.replace);

            //assert
            Assert.AreEqual("New Value", entity.Qux[0].Foo);
        }

        [TestMethod]
        public void SetValueFromPath_RemoveFromListItem_SetsValue()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Qux = new List<SimpleEntity>
                {
                    new SimpleEntity { Foo = "Old Value" }
                }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/qux/0/Foo", entity, null, JsonPatchOperationType.remove);

            //assert
            Assert.IsNull(entity.Qux[0].Foo);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_AddToListItemOutOfBounds_ThrowsException()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Qux = new List<SimpleEntity>
                {
                    new SimpleEntity()
                }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/Qux/1/Foo", entity, "New Value", JsonPatchOperationType.add);
        }

        [TestMethod]
        public void SetValueFromPath_ReplaceNestedArray_ReplacesValue()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Foo = new ArrayEntity
                {
                    Foo = new string[] { "Element One" }
                }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/foo/foo/0", entity, "Element One - Updated", JsonPatchOperationType.replace);

            //assert
            Assert.AreEqual("Element One - Updated", entity.Foo.Foo[0]);
        }

        [TestMethod, ExpectedException(typeof(JsonPatchException))]
        public void SetValueFromPath_AddToNestedArray_ThrowsException()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Foo = new ArrayEntity
                {
                    Foo = new string[] { "Element One" }
                }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/foo/Foo/1", entity, "New Value", JsonPatchOperationType.add);
        }

        [TestMethod]
        public void SetValueFromPath_AddToNestedList_AddsValue()
        {
            //arrange
            var entity = new ComplexEntity
            {
                Norf = new List<ListEntity>()
                {
                    new ListEntity
                    {
                        Foo = new List<string>()
                    }
                }
            };

            //act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/Norf/0/foo/0", entity, "Element One", JsonPatchOperationType.add);

            //assert
            Assert.AreEqual(1, entity.Norf[0].Foo.Count);
            Assert.AreEqual("Element One", entity.Norf[0].Foo[0]);
        }

        #endregion

        [TestMethod]
        public void SetValueFromPath_ComplexValue_SetsValue()
        {
            //arrange
            var entity = new ComplexEntity { };
            var value = new SimpleEntity
            {
                Foo = "I am foo",
                Bar = 12,
                Baz = "I am baz"
            };

            // act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/bar", entity, value, JsonPatchOperationType.add);

            //assert
            Assert.AreEqual("I am foo", entity.Bar.Foo);
            Assert.AreEqual(12, entity.Bar.Bar);
            Assert.AreEqual("I am baz", entity.Bar.Baz);
        }

        [TestMethod]
        public void SetValueFromPath_ComplexValueWithCollections_Sets()
        {
            //arrange
            var entity = new ComplexEntity { };
            var value = new ArrayEntity
            {
                Foo = new[] { "Element One", "Element Two", "Element Three" }
            };

            // act
            PathHelper.SetValueFromPath(typeof(ComplexEntity), "/foo", entity, value, JsonPatchOperationType.add);

            //assert
            Assert.AreEqual(3, entity.Foo.Foo.Length);
            Assert.AreEqual("Element One", entity.Foo.Foo[0]);
            Assert.AreEqual("Element Two", entity.Foo.Foo[1]);
            Assert.AreEqual("Element Three", entity.Foo.Foo[2]);
        }

        #endregion
    }
}
