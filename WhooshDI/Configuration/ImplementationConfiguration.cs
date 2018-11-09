using System;

namespace WhooshDI.Configuration
{
    public class ImplementationConfiguration
    {
        public Type ImplementationType { get; set; }
        public bool IsSingleton { get; set; }
        public object Name { get; set; }
    }
}