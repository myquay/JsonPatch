namespace JsonPatch;

/// <summary>
/// Json Patch Document interface
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IJsonPatchDocument<T> where T : class
{
    /// <summary>
    /// Apply the operations to the entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    T ApplyTo(T entity);
}