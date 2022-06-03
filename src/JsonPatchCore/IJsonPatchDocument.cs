
namespace JsonPatchCore;

public interface IJsonPatchDocument<T> where T : class
{
    public T ApplyTo(T entity);
}

