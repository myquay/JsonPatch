using JsonPatch.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Formatting
{
    public class JsonPatchFormatter : BufferedMediaTypeFormatter
    {
        public JsonPatchFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json-patch+json"));
        }

        public override bool CanWriteType(System.Type type)
        {
            return false;
        }

        public override bool CanReadType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(JsonPatchDocument<>))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object ReadFromStream(Type type, System.IO.Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            var entityType = type.GetGenericArguments()[0];

            using (StreamReader reader = new StreamReader(readStream))
            {
                var jsonPatchDocument = (IJsonPatchDocument)typeof(JsonPatchDocument<>)
                    .MakeGenericType(entityType)
                    .GetConstructor(Type.EmptyTypes)
                    .Invoke(null);

                var jsonString = reader.ReadToEnd();
                var operations = JsonConvert.DeserializeObject<PatchOperation[]>(jsonString);

                foreach (var operation in operations)
                {
                    if (operation.op == Constants.Operations.ADD)
                    {
                        jsonPatchDocument.Add(operation.path, operation.value);
                    }
                    else if (operation.op == Constants.Operations.REMOVE)
                    {
                        jsonPatchDocument.Remove(operation.path);
                    }
                    else if (operation.op == Constants.Operations.REPLACE)
                    {
                        jsonPatchDocument.Replace(operation.path,  operation.value);
                    }
                    else
                    {
                        throw new JsonPatchParseException(String.Format("The operation '{0}' is not supported.", operation.op));
                    }
                }

                return jsonPatchDocument;

            }
        }
    }
}
