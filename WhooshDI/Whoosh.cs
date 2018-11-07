using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WhooshDI.Attributes;
using WhooshDI.Configuration;

namespace WhooshDI
{
    public class Whoosh : IWhooshContainer
    {
        private readonly WhooshConfiguration _configuration;
        
        private readonly Dictionary<ImplementationConfiguration, object> _singletons = 
            new Dictionary<ImplementationConfiguration, object>();

        public Whoosh()
        {
        }

        public Whoosh(WhooshConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Resolve<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public T Resolve<T>(object name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (_configuration == null)
            {
                throw new InvalidOperationException("Configuration for container is not provided.");
            }

            var namedImplConfig = GetNamedImplementationConfiguration(typeof(T), name);

            return (T)GetInstance(typeof(T), namedImplConfig);
        }
        
        private object GetInstance(Type type)
        {
            var implConfig = _configuration != null ? GetImplementationConfiguration(type) : null;

            return GetInstance(type, implConfig);
        }

        private object GetInstance(Type type, ImplementationConfiguration implConfig)
        {
            var instance = TryGetInstanceIfSingleton(implConfig);
            if (instance != null)
            {
                return instance;
            }

            var typeToInstantiate = implConfig != null ? implConfig.ImplementationType : type;
            
            var constructor = typeToInstantiate.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();
            var arguments = GetConstructorArguments(constructor);
            
            instance = Activator.CreateInstance(typeToInstantiate, arguments);

            SaveInstanceIfSingleton(implConfig, instance);

            return instance;
        }

        private object[] GetConstructorArguments(ConstructorInfo constructor)
        {
            return constructor.GetParameters()
                .Select(param =>
                {
                    var dependencyKeyAttribute = param.GetCustomAttributes<DependencyKeyAttribute>().FirstOrDefault();

                    if (dependencyKeyAttribute == null)
                    {
                        return GetInstance(param.ParameterType);
                    }
                   
                    var namedImplConfig = GetNamedImplementationConfiguration(param.ParameterType, 
                        dependencyKeyAttribute.Key);

                    return GetInstance(param.ParameterType, namedImplConfig);
                })
                .ToArray();
        }

        private ImplementationConfiguration GetNamedImplementationConfiguration(Type type, object name)
        {
            var implConfigs = _configuration.GetConfigurationsForDependency(type) 
                ?? throw new InvalidOperationException($"Dependency {type.FullName} is not configured.");

            return implConfigs.FirstOrDefault(c => c.Name.Equals(name))
                ?? throw new InvalidOperationException(
                    $"Dependency {type.FullName} does not have implementation with name {name}.");
        }

        private ImplementationConfiguration GetImplementationConfiguration(Type type)
        {
            var implConfigs = _configuration.GetConfigurationsForDependency(type);
            
            return implConfigs == null ? null
                : implConfigs.Where(c => c.ImplementationType == type)?.First() ?? implConfigs.First();
        }

        private object TryGetInstanceIfSingleton(ImplementationConfiguration implConfig)
        {
            if (implConfig == null || implConfig.Lifestyle != Lifestyle.Singleton)
            {
                return null;
            } 
            
            return _singletons.TryGetValue(implConfig, out var singleton) ? singleton : null;
        }

        private void SaveInstanceIfSingleton(ImplementationConfiguration implConfig, object instance)
        {
            if (implConfig != null && implConfig.Lifestyle == Lifestyle.Singleton)
            {
                _singletons.Add(implConfig, instance);
            }
        }
    }
}