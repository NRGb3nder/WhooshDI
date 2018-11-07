using System;

namespace WhooshDI.Configuration
{
    public class ImplementationConfiguration
    {
        public Type ImplementationType { get; set; }
        public Lifestyle Lifestyle { get; set; }
        public object Name { get; set; }
    }
}