using System.Reflection;

namespace JsonPatch.Paths.Components
{
    public class PropertyPathComponent : PathComponent
    {
        public PropertyPathComponent(string name)
            : base(name)
        {
        }

        public PropertyInfo PropertyInfo { get; set; }
    }
}
