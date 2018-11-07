using System;
using System.Collections.Generic;
using WhooshDI.Configuration;

namespace WhooshDI
{
    public interface IWhooshConfiguration
    {
        List<ImplementationConfiguration> GetConfigurationsForDependency(Type type);
    }
}