using System.Reflection;

namespace JsonPatch.Paths.Components
{
    /// <summary>
    /// Property path component.
    /// </summary>
    public class PropertyPathComponent : PathComponent
    {
        /// <summary>
        /// Component name.
        /// </summary>
        /// <param name="name"></param>
        public PropertyPathComponent(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Returns the property info of the property represented by this component.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PropertyInfo GetPropertyInfo(object entity)
        {
            return entity.GetType().GetProperty(Name);
        }
    }
}
