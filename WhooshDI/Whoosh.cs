using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WhooshDI.Attributes;
using WhooshDI.Configuration;
using WhooshDI.Exceptions;

namespace WhooshDI
{
    public class Whoosh : IWhooshContainer
    {
        private readonly WhooshConfiguration _configuration;
        
        private readonly Lazy<Dictionary<ImplementationConfiguration, object>> _singletons = 
            new Lazy<Dictionary<ImplementationConfiguration, object>>();

        private readonly Stack<Type> _trace = new Stack<Type>();

        public Whoosh()
        {
        }

        public Whoosh(WhooshConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
            if (_trace.Contains(type))
            {
                throw new CircularDependencyException(
                    $"Circular dependency detected:\n{string.Join(" in\n", _trace.Select(e => e.FullName).ToArray())}");
            }
            
            _trace.Push(type);
            
            var instance = TryGetInstanceIfSingleton(implConfig);
            if (instance != null)
            {
                return instance;
            }

            var typeToInstantiate = implConfig != null ? implConfig.ImplementationType : type;

            var constructor = GetConstructorWithLongestParameterList(typeToInstantiate)
                ?? throw new InvalidOperationException($"Could not instantiate type: {typeToInstantiate.FullName}");
            var arguments = GetConstructorArguments(constructor);
            
            instance = Activator.CreateInstance(typeToInstantiate, arguments);

            SaveInstanceIfSingleton(implConfig, instance);

            _trace.Pop();

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

            return implConfigs.FirstOrDefault(c => c.Name != null && c.Name.Equals(name))
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
            
            return _singletons.Value.TryGetValue(implConfig, out var singleton) ? singleton : null;
        }

        private void SaveInstanceIfSingleton(ImplementationConfiguration implConfig, object instance)
        {
            if (implConfig != null && implConfig.Lifestyle == Lifestyle.Singleton)
            {
                _singletons.Value.Add(implConfig, instance);
            }
        }
        
        private static ConstructorInfo GetConstructorWithLongestParameterList(Type type)
        {
            return type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();
        }
    }
}