using System;
using System.Collections.Generic;
using WhooshDI.Configuration;

namespace WhooshDI
{
    /// <summary>
    /// Configuration for <see cref="Whoosh"/> DI container.
    /// </summary>
    public interface IWhooshConfiguration
    {
        /// <summary>
        /// Validates configuration contents.
        /// </summary>
        void Validate();
        
        /// <summary>
        /// Gets a configurations for all registered dependency implementations.
        /// </summary>
        /// <param name="type">A type of a dependency.</param>
        /// <returns>A <c>List</c> of <see cref="ImplementationConfiguration"/> for each registered implementation.</returns>
        List<ImplementationConfiguration> GetConfigurationsForDependency(Type type);
    }
}