using System;

namespace WhooshDI.Configuration
{
    /// <summary>
    /// Configuration of a dependency implementation.
    /// </summary>
    public class ImplementationConfiguration
    {
        /// <summary>
        /// Gets or sets a <see cref="Type"/> of an implementation.
        /// </summary>
        public Type ImplementationType { get; set; }
        
        /// <summary>
        /// Gets or sets user-defined instance of an object.
        /// </summary>
        public object Instance { get; set; }
        
        /// <summary>
        /// Gets or sets a flag that indicates if implementation will be instantiated as a singleton.
        /// </summary>
        public bool IsSingleton { get; set; }
        
        /// <summary>
        /// Gets or sets a unique name of an implementation.
        /// </summary>
        public object Name { get; set; }
    }
}