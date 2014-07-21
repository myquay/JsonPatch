JsonPatch
=========

JsonPatch is a simple library which adds JSON Patch support to ASP.NET Web API (http://tools.ietf.org/html/rfc6902).

Usage
=========

1. Install the formatter

    public static void ConfigureApis(HttpConfiguration config)
    {
        config.Formatters.Add(new JsonPatchFormatter());
    }

2. Profit??

    public void Patch(Guid id, JsonPatchDocument<SomeDto> patchData)
    {
        //Remember to do some validation and all that fun stuff
        
        var objectToUpdate = repository.GetById(id);
        patchData.ApplyUpdatesTo(objectToUpdate);
        repository.Save(objectToUpdate);
    }
    
Notes
=========

This is still very much an early project, don't use it in production unless you understand the source and don't mind fixing a few bugs ;)

Michael 
@myquay
