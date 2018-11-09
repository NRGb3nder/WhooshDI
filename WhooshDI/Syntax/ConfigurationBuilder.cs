using System;
using WhooshDI.Configuration;

namespace WhooshDI.Syntax
{
    public class ConfigurationBuilder : IFluentSyntax
    {
        private readonly ImplementationConfiguration _config;

        public ConfigurationBuilder(ImplementationConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
            
        public ConfigurationBuilder AsSingleton()
        {
            _config.IsSingleton = true;
        
            return this;
        }

        public ConfigurationBuilder AsTransient()
        {
            _config.IsSingleton = false;

            return this;
        }

        public ConfigurationBuilder WithName(object name)
        {
            _config.Name = name ?? throw new ArgumentNullException(nameof(name));

            return this;
        }
    }
}