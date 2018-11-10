using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Force.DeepCloner;
using WhooshDI.Attributes;
using WhooshDI.Configuration;
using WhooshDI.Exceptions;
using WhooshDI.Resources;

namespace WhooshDI
{
    /// <inheritdoc/>
    /// <summary>
    /// Simple dependency injection container with separated configuration that supports
    /// Transient and Singleton dependency lifestyles, constructor and property injection types,
    /// named dependencies and open generic dependencies.
    /// </summary>
    public class Whoosh : IWhooshContainer
    {
        // TODO: Lists -> collection interface
        // TODO: Conditions
        // TODO: Scoped singleton and transient dependencies
        // TODO: User-created instance
        // TODO: Collections
        // TODO: null checks -> aspect (CastleCore)
        private readonly IWhooshConfiguration _configuration;
        
        private readonly Dictionary<ImplementationConfiguration, object> _singletons = 
            new Dictionary<ImplementationConfiguration, object>();
        
        private readonly object _syncRoot = new object();

        private readonly Stack<Type> _trace = new Stack<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Whoosh"/> container.
        /// </summary>
        public Whoosh()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Whoosh"/> container.
        /// </summary>
        /// <param name="configuration">An <see cref="IWhooshConfiguration"/> object with resolution rules</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <c>null</c></exception>
        public Whoosh(IWhooshConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            
            configuration.Validate();

            _configuration = configuration.DeepClone();
        }

        public T Resolve<T>()
        {
            lock (_syncRoot)
            {
                return (T)GetInstance(typeof(T));
            }
        }

        public T Resolve<T>(object name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (_configuration == null)
            {
                throw new InvalidOperationException(ExceptionMessages.InvalidOperationExceptionNoConfiguration);
            }

            var namedImplConfig = GetNamedImplementationConfiguration(typeof(T), name);

            lock (_syncRoot)
            {
                return (T)GetInstance(typeof(T), namedImplConfig);
            }
        }
        
        private object GetInstance(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return GetAllImplementationsCollection(type);
            }

            var implConfig = GetImplementationConfiguration(type);
            if (implConfig == null && type.IsGenericType)
            {
                implConfig = GetImplementationConfiguration(type.GetGenericTypeDefinition());
            }

            return GetInstance(type, implConfig);
        }

        private object GetInstance(Type type, ImplementationConfiguration implConfig)
        {
            if (_trace.Contains(type))
            {
                throw new CircularDependencyException(
                    ExceptionMessages.CircularDependencyExceptionIsDetected
                        .Replace("{trace}", string.Join(" in\n", _trace.Select(e => e.FullName).ToArray())));
            }
            
            _trace.Push(type);
            
            var instance = TryGetInstanceIfSingleton(implConfig);
            if (instance != null)
            {
                return instance;
            }

            var typeToInstantiate = implConfig != null ? implConfig.ImplementationType : type;

            if (typeToInstantiate.IsGenericTypeDefinition)
            {
                typeToInstantiate = typeToInstantiate.MakeGenericType(type.GenericTypeArguments);
            }

            var constructor = GetConstructorWithLongestParameterList(typeToInstantiate)
                ?? throw new InvalidOperationException(ExceptionMessages.InvalidOperationExceptionCantInstantiateType
                    .Replace("{type}", typeToInstantiate.FullName));
            var arguments = GetConstructorArguments(constructor);
            
            instance = Activator.CreateInstance(typeToInstantiate, arguments);

            ResolvePropertyDependencies(type, instance);

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
                ?? throw new InvalidOperationException(ExceptionMessages.InvalidOperationExceptionDependencyNotConfigured
                    .Replace("{dependency}", type.FullName));

            return implConfigs.FirstOrDefault(c => c.Name != null && c.Name.Equals(name))
                ?? throw new InvalidOperationException(
                    ExceptionMessages.InvalidOperationExceptionNoImplementationWithSuchName
                        .Replace("{dependency}", type.FullName)
                        .Replace("{name}", name.ToString()));
        }

        private ImplementationConfiguration GetImplementationConfiguration(Type type)
        {
            var implConfigs = _configuration?.GetConfigurationsForDependency(type);

            if (implConfigs == null)
            {
                return null;
            }

            if (implConfigs.Count > 1)
            {
                throw new InvalidOperationException(
                    ExceptionMessages.InvalidOperationExceptionAmbiguityInDependencyResolution
                        .Replace("{dependency}", type.FullName));
            }
            
            return implConfigs.First();
        }

        private IEnumerable GetAllImplementationsCollection(Type type)
        {
            var genericArgument = type.GenericTypeArguments.First();
            var implConfigs = _configuration.GetConfigurationsForDependency(genericArgument);

            if (implConfigs == null)
            {
                throw new InvalidOperationException(
                    ExceptionMessages.InvalidOperationExceptionDependencyNotConfigured
                        .Replace("{dependency}", genericArgument.FullName));
            }

            var allImplementationsInstances = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericArgument));
            foreach (var config in implConfigs)
            {
                allImplementationsInstances.Add(GetInstance(genericArgument, config));
            }
                
            return allImplementationsInstances;
        }

        private void ResolvePropertyDependencies(Type type, object instance)
        {
            foreach (var property in type.GetProperties())
            {
                if (property.GetCustomAttributes<WhooshResolveAttribute>().FirstOrDefault() != null)
                {
                    if (property.GetSetMethod() == null)
                    {
                        throw new InvalidOperationException(
                            ExceptionMessages.InvalidOperationExceptionPropertySetterNotAccessible
                                .Replace("{property}", property.Name)
                                .Replace("{type}", type.FullName));
                    }
                    
                    property.SetValue(instance, GetInstance(property.PropertyType));
                }
            }
        }

        private object TryGetInstanceIfSingleton(ImplementationConfiguration implConfig)
        {
            if (implConfig == null || implConfig.IsSingleton != true)
            {
                return null;
            } 
            
            return _singletons.TryGetValue(implConfig, out var singleton) ? singleton : null;
        }

        private void SaveInstanceIfSingleton(ImplementationConfiguration implConfig, object instance)
        {
            if (implConfig != null && implConfig.IsSingleton == true)
            {
                _singletons.Add(implConfig, instance);
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