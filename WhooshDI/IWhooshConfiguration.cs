using System;
using System.Collections.Generic;
using WhooshDI.Configuration;

namespace WhooshDI
{
    public interface IWhooshConfiguration
    {
        IEnumerable<ImplementationConfiguration> GetConfiguration(Type type);
    }
}