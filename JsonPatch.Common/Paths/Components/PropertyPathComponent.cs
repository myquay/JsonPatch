using System.Reflection;

namespace JsonPatch.Common.Paths.Components
{
    public class PropertyPathComponent : PathComponent
    {
        public PropertyPathComponent(string name)
            : base(name)
        {
        }

        public PropertyInfo GetPropertyInfo(object entity)
        {
            return entity.GetType().GetProperty(Name);
        }
    }
}
