using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Helpers
{
    public static class DiffHelper
    {
        public static IEnumerable<JsonPatchOperation> GenerateDiff(object originalDocument, object modifiedDocument, string path = "/")
        {
            if (originalDocument.GetType() != modifiedDocument.GetType())
                throw new ArgumentException(string.Format("Original Document is type of {0} but Modified Document is of type {1}", originalDocument.GetType(), modifiedDocument.GetType()));

            //If it's just a value type or a string, then we can compare them here without going any further. 
            if (originalDocument.GetType().IsValueType || originalDocument.GetType() == typeof(string))
            {
                if (originalDocument != modifiedDocument)
                {
                    yield return new JsonPatchOperation()
                    {
                        Operation = modifiedDocument == null ? JsonPatchOperationType.remove : JsonPatchOperationType.replace,
                        PropertyName = path,
                        Value = modifiedDocument
                    };
                }
                yield break;
            }
       
            var propertyList = originalDocument.GetType().GetProperties();
            foreach (var property in propertyList)
            {
                var originalValue = property.GetValue(originalDocument);
                var modifiedValue = property.GetValue(modifiedDocument);

                if (property.PropertyType.IsArray)
                {
                    var originalArrayValue = originalValue as Array;
                    var modifiedArrayValue = modifiedValue as Array;

                    //Array add/remove
                    foreach (var addRemoveDiff in ArrayAddRemoveDiff(originalArrayValue,  modifiedArrayValue, path + property.Name + "/"))
                        yield return addRemoveDiff;

                    //Compare each object in the array recursively. 
                    var compareCount = Math.Min(originalArrayValue.Length,modifiedArrayValue.Length);
                    for (int i = 0; i < compareCount; i++)
                    {
                        foreach (var itemDiff in GenerateDiff(originalArrayValue.GetValue(i), modifiedArrayValue.GetValue(i), path + property.Name + "/" + i))
                            yield return itemDiff;
                    }

                }else if (property.PropertyType.IsValueType || property.PropertyType != typeof(string))
                {
                    //Nested object. 
                    foreach (var patchOperation in GenerateDiff(originalValue, modifiedValue, path + property.Name + "/"))
                        yield return patchOperation;
                }else if (originalValue != modifiedValue)
                {
                    //Standard Value Type/String
                    yield return new JsonPatchOperation()
                    {
                        Operation = modifiedValue == null ? JsonPatchOperationType.remove : JsonPatchOperationType.replace,
                        PropertyName = path + property.Name,
                        Value = property.GetValue(modifiedDocument)
                    };
                }
            }

            yield break;
        }

        private static IEnumerable<JsonPatchOperation> ArrayAddRemoveDiff(Array originalArray, Array modifiedArray, string path)
        {
            if (originalArray.Length > modifiedArray.Length)
            {
                var removeCount = originalArray.Length - modifiedArray.Length;
                var count = 0;
                while (count < removeCount)
                {
                    yield return new JsonPatchOperation()
                    {
                        Operation = JsonPatchOperationType.remove,
                        PropertyName = path + (originalArray.Length - 1 - count).ToString()
                    };
                    count++;
                }
            }
            else if(originalArray.Length < modifiedArray.Length)
            {
                var addCount = modifiedArray.Length - originalArray.Length;
                var count = 0;
                while (count < addCount)
                {
                    yield return new JsonPatchOperation()
                    {
                        Operation = JsonPatchOperationType.add,
                        PropertyName = path + (originalArray.Length + count).ToString(), 
                        Value = modifiedArray.GetValue(originalArray.Length + count)
                    };
                    count++;
                }
            }
            yield break;
        }
    }
}
