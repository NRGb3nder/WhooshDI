using System;
using System.Collections.Generic;
using System.Linq;
using WhooshDI.Configuration;
using WhooshDI.Exceptions;
using WhooshDI.Syntax;

namespace WhooshDI
{
    public abstract class WhooshConfiguration : IWhooshConfiguration
    {
        private readonly Dictionary<Type, List<ImplementationConfiguration>> _dependencyConfigurations = 
            new Dictionary<Type, List<ImplementationConfiguration>>();

        public void Validate()
        {
            var duplicateImplementationExceptions = new List<DuplicateElementException>();
            var duplicateNameExceptions = new List<DuplicateElementException>();

            _dependencyConfigurations.Keys.ToList().ForEach(dependency =>
            {
                _dependencyConfigurations[dependency]
                    .GroupBy(c => c.ImplementationType)
                    .Where(c => c.Count() > 1)
                    .ToList()
                    .ForEach(group => duplicateImplementationExceptions.Add(
                        new DuplicateElementException(
                            $"Duplicate implementation {group.Key.FullName} of dependency {dependency.FullName}")));

                _dependencyConfigurations[dependency]
                    .GroupBy(c => c.Name)
                    .Where(c => c.Count() > 1)
                    .ToList()
                    .ForEach(group => duplicateNameExceptions.Add(
                        new DuplicateElementException(
                            group.Key != null
                                ? $"Duplicate name {group.Key} in implementations of dependency {dependency.FullName}"
                                : $"Duplicate unnamed implementations of dependency {dependency.FullName}")));
            });

            var exceptions = duplicateImplementationExceptions.Concat(duplicateNameExceptions).ToList();
            switch (exceptions.Count)
            {
                case 0:
                    return;
                case 1:
                    throw exceptions.First();
                default:
                    throw new AggregateException(exceptions);
            }
        }
        
        public List<ImplementationConfiguration> GetConfigurationsForDependency(Type type)
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