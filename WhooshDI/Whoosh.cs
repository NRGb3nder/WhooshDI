using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using WhooshDI.Configuration;

namespace WhooshDI
{
    public class Whoosh : IWhooshContainer
    {
        private readonly WhooshConfiguration _configuration;
        
        private readonly Dictionary<ImplementationConfiguration, object> _singletones = 
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
            return (T) GetInstance(typeof(T));
        }

        public T Resolve<T>(int name)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(string name)
        {
            throw new NotImplementedException();
        }
        
        private object GetInstance(Type type)
        {
            var implConfig = _configuration != null ? GetImplementationConfiguration(type) : null;

            var instance = GetInstanceIfSingleton(implConfig);
            if (instance != null)
            {
                return instance;
            }
            
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();
            var arguments = constructor.GetParameters()
                .Select(param => GetInstance(param.ParameterType))
                .ToArray();
            
            instance = Activator.CreateInstance(type, arguments);

            SaveInstanceIfSingleton(implConfig, instance);

            return instance;
        }

        private ImplementationConfiguration GetImplementationConfiguration(Type type)
        {
            var implConfigs = _configuration.GetConfigurationsForDependency(type);
            
            return implConfigs == null ? null
                : implConfigs.Where(c => c.ImplementationType == type)?.First() ?? implConfigs.First();
        }

        private object GetInstanceIfSingleton(ImplementationConfiguration implConfig)
        {
            if (implConfig == null || implConfig.Lifestyle != Lifestyle.Singleton)
            {
                return null;
            } 
            
            return _singletones.TryGetValue(implConfig, out var singleton) ? singleton : null;
        }

        private void SaveInstanceIfSingleton(ImplementationConfiguration implConfig, object instance)
        {
            if (implConfig != null && implConfig.Lifestyle == Lifestyle.Singleton)
            {
                _singletones.Add(implConfig, instance);
            }
        }
    }
}