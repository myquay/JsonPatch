JsonPatch
=========

JsonPatch is a simple library which adds JSON Patch support to .NET 6 - ASP.NET Core Minimal Web API (http://tools.ietf.org/html/rfc6902).

You can get it on NuGet here: https://www.nuget.org/packages/JsonPatch/

Usage
=========

## Step 1: Specify the Patch Document

```C#

endpoints.MapMethods("/some-resource/{id}", new[] { "PATCH" }, async (Guid id, JsonPatchDocument<SomeDto> model) =>
{
    //Remember to do some validation and all that fun stuff
    var objectToUpdate = repository.GetById(id);
    repository.Save(patchData.ApplyTo(objectToUpdate));
});

```

Yep - that's it. You can now use PATCH on your flash new Minimal API (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0)

## Making a PATCH request

The main thing is to make sure the content type is "application/json-patch+json" otherwise Web API won't invoke the JsonPatch media formatter. Here's an example.

    PATCH /my/data HTTP/1.1
    Host: example.org
    Content-Type: application/json-patch+json
    
    [
        { "op": "add", "path": "/a/b/c", "value": "foo" }
    ]

## Options

The path specified in the patch request can be resolved to a different property on the model using a resolver.

E.g. the ExactCasePropertyPathResolver will match a property based on an exact match

```C#

JsonPatchSettings.Options = new JsonPatchOptions
{
    PathResolver = new ExactCasePropertyPathResolver(new JsonValueConverter()),
    RequireJsonPatchContentType = false,
};

```

Now the request 

    PATCH /my/data HTTP/1.1
    Host: example.org
    Content-Type: application/json-patch+json
    
    [
        { "op": "add", "path": "/sampleProperty", "value": "foo" }
    ]
    
Will be no longer be valid for the class

    {
        [JsonProperty("sampleProperty")]
        public string SampeProperty {get;set;}
    }

Available resolvers are
* ExactCasePropertyPathResolver
* CaseInsensitivePropertyPathResolver
* AttributePropertyPathResolver
* FlexiblePathResolver

You can add your own by extending BaseResolver and defining how to get the property from the type. 

Notes
=========

Because we are applying updates to C# objects rather than JSON objects we differ from the spec slightly, here are some points of difference.

### JSON Pointer [(rfc6901)](http://tools.ietf.org/html/rfc6901)

The JSON patch document uses JSON pointers to refer to properties, we don't accept paths in the following formats:

1. The path __""__ is invalid as it refers to the whole object, if you're replacing everything, just use PUT.
2. The path __"/"__ is invalid as you cannot have an empty property.
3. The path __"/5"__ is invalid as you can only update an entity, not an array of entities.

### Available operations [(rfc6902#section-4)](http://tools.ietf.org/html/rfc6902#section-4)

We only support a subset of available operations at this moment, add, remove, and replace.

Since C# is a typed language we have a few restrictions, hence the operations work a little differently than specified in the specification.

Here's our flavour:

1. Add will not add a property that does not exist, it can only be used to specify values on properties that are null.
2. Remove will not remove a property, it will set its value to null.
3. Replace will replace the value of a property even if it is null.
