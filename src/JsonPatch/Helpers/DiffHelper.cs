using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Helpers
{
    public static class DiffHelper
    {
        public static IEnumerable<JsonPatchOperation> GenerateDiff(Type entity, object originalDocument, object modifiedDocument, string path = "/")
        {
            if (originalDocument.GetType() != entity)
                throw new ArgumentException(string.Format("Original Document is type of {0} but requested type is {1}", originalDocument.GetType().ToString(), entity.ToString()));

            if (modifiedDocument.GetType() != entity)
                throw new ArgumentException(string.Format("Modified Document is type of {0} but requested type is {1}", modifiedDocument.GetType().ToString(), entity.ToString()));

            var propertyList = entity.GetProperties();
            foreach (var property in propertyList)
            {
                var originalValue = property.GetValue(originalDocument);
                var modifiedValue = property.GetValue(modifiedDocument);

                if (property.PropertyType.IsArray)
                {
                    //Array
                    foreach (var addRemoveDiff in ArrayAddRemoveDiff(originalValue as Array, modifiedValue as Array, path + property.Name + "/"))
                        yield return addRemoveDiff;
                }else if (property.PropertyType.IsValueType || property.PropertyType != typeof(string))
                {
                    //Nested object. 
                    foreach (var patchOperation in GenerateDiff(property.PropertyType, originalValue, modifiedValue, path + property.Name + "/"))
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
