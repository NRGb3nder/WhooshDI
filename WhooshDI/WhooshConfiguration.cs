using System;
using System.Collections.Generic;
using System.Linq;
using WhooshDI.Configuration;
using WhooshDI.Syntax;

namespace WhooshDI
{
    public abstract class WhooshConfiguration : IWhooshConfiguration
    {
        private readonly Dictionary<Type, List<ImplementationConfiguration>> _dependencyConfigurations = 
            new Dictionary<Type, List<ImplementationConfiguration>>();

        public IEnumerable<ImplementationConfiguration> GetConfiguration(Type type)
        {
            return _dependencyConfigurations.TryGetValue(type, out var configuration) ? configuration : null;
        }

        protected ConfigurationBuilder Register<TDependency, TImplementation>() 
            where TDependency : class
            where TImplementation : TDependency
        {
            var newConfig = new ImplementationConfiguration()
            {
                ImplementationType = typeof(TImplementation),
                Lifestyle = Lifestyle.Transient
            };
            
            RegisterImplementation(typeof(TDependency), newConfig);

            return new ConfigurationBuilder(newConfig);
        }

        protected ConfigurationBuilder Register<T>() where T : class
        {
            var newConfig = new ImplementationConfiguration()
            {
                ImplementationType = typeof(T),
                Lifestyle = Lifestyle.Transient
            };
            
            RegisterImplementation(typeof(T), newConfig);

            return new ConfigurationBuilder(newConfig);
        }
        
        private void RegisterImplementation(Type dependency, ImplementationConfiguration config)
        {
            if (_dependencyConfigurations.TryGetValue(dependency, out var implConfigs))
            {
                implConfigs.Add(config);
            }
            else
            {
                _dependencyConfigurations.Add(dependency, new List<ImplementationConfiguration>()
                {
                    config
                });
            }
        }
    }
}