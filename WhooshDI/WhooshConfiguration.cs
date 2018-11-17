using System;
using System.Collections.Generic;
using System.Linq;
using WhooshDI.Configuration;
using WhooshDI.Exceptions;
using WhooshDI.Internal;
using WhooshDI.Syntax;
using WhooshDI.Resources;

namespace WhooshDI
{
    /// <inheritdoc/>
    /// <summary>
    /// Abstract configuration that provides resolution rules definition methods for derived configurations.
    /// </summary>
    /// <remarks>User-defined container configurations should inherit from this type.</remarks>
    public abstract class WhooshConfiguration : IWhooshConfiguration
    {
        private readonly Dictionary<Type, ICollection<ImplementationConfiguration>> _dependencyConfigurations = 
            new Dictionary<Type, ICollection<ImplementationConfiguration>>();

        public void Validate()
        {
            var duplicateImplementationExceptions = new List<DuplicateElementException>();
            var duplicateNameExceptions = new List<DuplicateElementException>();

            foreach (var dependency in _dependencyConfigurations.Keys)
            {
                duplicateImplementationExceptions.AddRange(_dependencyConfigurations[dependency]
                    .GroupBy(c => c.ImplementationType)
                    .Where(c => c.Count() > 1)
                    .Select(group => new DuplicateElementException(
                        ExceptionMessages.DuplicateElementExceptionDuplicateImplementation
                            .Replace("{implementation}", group.Key.FullName)
                            .Replace("{dependency}", dependency.FullName))));

                duplicateNameExceptions.AddRange(_dependencyConfigurations[dependency]
                    .GroupBy(c => c.Name)
                    .Where(c => c.Key != null && c.Count() > 1)
                    .Select(group => new DuplicateElementException(
                        ExceptionMessages.DuplicateElementExceptionDuplicateImplementationName
                            .Replace("{name}", group.Key.ToString())
                            .Replace("{dependency}", dependency.FullName))));
            }

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
        
        public ICollection<ImplementationConfiguration> GetConfigurationsForDependency(Type type)
        {
            return _dependencyConfigurations.TryGetValue(type, out var configuration) ? configuration : null;
        }
        
        public void Dispose()
        {
            foreach (var configs in _dependencyConfigurations.Values)
            {
                foreach (var config in configs)
                {
                    (config.Instance as IDisposable)?.Dispose();
                }
            }
        }

        /// <summary>
        /// Registers a new implementation for a dependency.
        /// </summary>
        /// <typeparam name="TDependency">A dependency to resolve</typeparam>
        /// <typeparam name="TImplementation">An implementation for a dependency</typeparam>
        /// <returns></returns>
        protected ConfigurationBuilder Register<TDependency, TImplementation>()
            where TDependency : class
            where TImplementation : TDependency
        {
            var newConfig = new ImplementationConfiguration
            {
                ImplementationType = typeof(TImplementation)
            };
            
            RegisterImplementation(typeof(TDependency), newConfig);

            return new ConfigurationBuilder(newConfig);
        }

        /// <summary>
        /// Registers a new dependency.
        /// </summary>
        /// <typeparam name="T">A dependency to configure</typeparam>
        /// <returns></returns>
        protected ConfigurationBuilder Register<T>() where T : class
        {
            var newConfig = new ImplementationConfiguration
            {
                ImplementationType = typeof(T)
            };
            
            RegisterImplementation(typeof(T), newConfig);

            return new ConfigurationBuilder(newConfig);
        }

        /// <summary>
        /// Registers a user-defined instance for a dependency.
        /// </summary>
        /// <param name="instance">A user-defined instance to resolve dependency</param>
        /// <typeparam name="T">A dependency to configure</typeparam>
        /// <returns></returns>
        protected ConfigurationBuilder Register<T>(T instance) where T : class
        {
            var newConfig = new ImplementationConfiguration()
            {
                ImplementationType = typeof(T),
                Instance = instance
            };
            
            RegisterImplementation(typeof(T), newConfig);
            
            return new ConfigurationBuilder(newConfig);
        }

        /// <summary>
        /// Registers a new implementation for a dependency.
        /// </summary>
        /// <param name="dependency">A dependency to resolve</param>
        /// <param name="implementation">An implementation for a dependency</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="dependency"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">Throws when <paramref name="implementation"/> is <c>null</c></exception>
        protected ConfigurationBuilder Register(Type dependency, Type implementation)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            if (dependency.IsValueType)
            {
                throw new ArgumentException(ExceptionMessages.ArgumentExceptionNotAReferenceType
                    .Replace("{type}", dependency.FullName));
            }

            if (!dependency.IsAssignableFrom(implementation) && !implementation.IsAssignableToGenericType(dependency))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentExceptionTypeIsNotAssignable
                    .Replace("{firstType}", implementation.FullName)
                    .Replace("{secondType}", dependency.FullName));
            }
            
            var newConfig = new ImplementationConfiguration
            {
                ImplementationType = implementation
            };
            
            RegisterImplementation(dependency, newConfig);
            
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