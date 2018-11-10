using System;

namespace WhooshDI.Attributes
{
    /// <summary>
    /// Attribute that is used to provide an implementation name for a constructor dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute : Attribute
    {
        /// <summary>
        /// Gets a name of a dependency implementation.
        /// </summary>
        public object Key { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyKeyAttribute"/> class.
        /// </summary>
        /// <param name="key">A name of a dependency implementation</param>
        public DependencyKeyAttribute(object key) => Key = key;
    }
}