using System;
using System.Collections.Generic;
using WhooshDI.Configuration;

namespace WhooshDI
{
    public interface IWhooshConfiguration
    {
        void Validate();
        
        List<ImplementationConfiguration> GetConfigurationsForDependency(Type type);
    }
}