using System;
using WhooshDI.Configuration;

namespace WhooshDI.Syntax
{
    /// <summary>
    /// Builder for a dependency resolution rule.
    /// </summary>
    public class ConfigurationBuilder : IFluentSyntax
    {
        private readonly ImplementationConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBuilder"/> class.
        /// </summary>
        /// <param name="config">An <see cref="ImplementationConfiguration"/> to configure</param>
        public ConfigurationBuilder(ImplementationConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        /// <summary>
        /// Configures dependency to be resolved with a singleton implementation.
        /// </summary>
        /// <returns></returns>
        public ConfigurationBuilder AsSingleton()
        {
            _config.IsSingleton = true;
        
            return this;
        }

        /// <summary>
        /// Configures dependency to be resolved with a new instance of implementation.
        /// </summary>
        /// <returns></returns>
        public ConfigurationBuilder AsTransient()
        {
            _config.IsSingleton = false;

            return this;
        }

        /// <summary>
        /// Allows implementation to be referenced with specified name.
        /// </summary>
        /// <param name="name">A name of an implementation</param>
        /// <returns></returns>
        public ConfigurationBuilder WithName(object name)
        {
            _config.Name = name ?? throw new ArgumentNullException(nameof(name));

            return this;
        }
    }
}