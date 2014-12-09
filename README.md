JsonPatch
=========

JsonPatch is a simple library which adds JSON Patch support to ASP.NET Web API (http://tools.ietf.org/html/rfc6902).

You can get it on NuGet here: https://www.nuget.org/packages/JsonPatch/

Usage
=========

##Step 1: Install the formatter


```C#
public static void ConfigureApis(HttpConfiguration config)
{
    config.Formatters.Add(new JsonPatchFormatter());
}
```

##Step 2: Profit??

```C#
public void Patch(Guid id, JsonPatchDocument<SomeDto> patchData)
{
    //Remember to do some validation and all that fun stuff
    var objectToUpdate = repository.GetById(id);
    patchData.ApplyUpdatesTo(objectToUpdate);
    repository.Save(objectToUpdate);
}
```

##Making a PATCH request

The main thing is to make sure the content type is "application/json-patch+json" otherwise Web API won't invoke the JsonPatch media formatter. Here's an example.

    PATCH /my/data HTTP/1.1
    Host: example.org
    Content-Type: application/json-patch+json
    
    [
        { "op": "add", "path": "/a/b/c", "value": "foo" }
    ]

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

This is still very much an early project, don't use it in production yet unless you understand the source and don't mind fixing a few bugs ;)

Twitter: @myquay
