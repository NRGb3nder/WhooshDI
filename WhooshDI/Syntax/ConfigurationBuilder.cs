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
            _config.Lifestyle = Lifestyle.Singleton;
        
            return this;
        }

        public ConfigurationBuilder AsTransient()
        {
            _config.Lifestyle = Lifestyle.Transient;

            return this;
        }

        public ConfigurationBuilder WithName(int name)
        {
            _config.Name = name;

            return this;
        }

        public ConfigurationBuilder WithName(string name)
        {
            _config.Name = name ?? throw new ArgumentNullException(nameof(name));

            return this;
        }
    }
}