using System;
using System.Linq;

namespace WhooshDI
{
    public class Whoosh : IWhooshContainer
    {
        private readonly WhooshConfiguration _configuration;

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
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            var arguments = constructor.GetParameters()
                .Select(param => GetInstance(param.ParameterType))
                .ToArray();
            
            return Activator.CreateInstance(type, arguments);
        }
    }
}