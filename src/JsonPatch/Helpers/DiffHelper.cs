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

                if (originalValue != modifiedValue)
                {
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
    }
}
