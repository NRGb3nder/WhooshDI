namespace WhooshDI.UnitTests.Helpers
{
    internal class TransientImplementationConfiguration : WhooshConfiguration
    {
        public TransientImplementationConfiguration() => Register<ClassWithParameterlessCtor>().AsTransient();
    }
    
    internal class SingletonImplementationConfiguration : WhooshConfiguration
    {
        public SingletonImplementationConfiguration() => Register<ClassWithParameterlessCtor>().AsSingleton();
    }

    internal class TransportProtocolsConfiguration : WhooshConfiguration
    {
        public TransportProtocolsConfiguration()
        {
            Register<ITransportLayerProtocol, TcpProtocol>().WithName("TCP");
            Register<ITransportLayerProtocol, UdpProtocol>().WithName("UDP");
        }
    }

    internal class TaxiConfiguration : WhooshConfiguration
    {
        public TaxiConfiguration() => Register<ICar, RenaultCar>();
    }

    internal class ConfigurationWithNamedDependencies : WhooshConfiguration
    {
        public ConfigurationWithNamedDependencies()
        {
            Register<ICar, RenaultCar>().WithName(Cars.Renault);
            Register<ICar, VolkswagenCar>().WithName(Cars.Volkswagen);
        }
    }

    internal class ConfigurationWithoutDependencyNames : WhooshConfiguration
    {
        public ConfigurationWithoutDependencyNames()
        {
            Register<ITransportLayerProtocol, TcpProtocol>();
            Register<ITransportLayerProtocol, UdpProtocol>();
        }
    }
    
    internal class ConfigurationWithDuplicateImplementations : WhooshConfiguration
    {
        public ConfigurationWithDuplicateImplementations()
        {
            Register<ITransportLayerProtocol, TcpProtocol>();
            Register<ITransportLayerProtocol, TcpProtocol>().WithName("Definitely a TCP");
        }
    }

    internal class ConfigurationWithDuplicateNames : WhooshConfiguration
    {
        public ConfigurationWithDuplicateNames()
        {
            Register<ITransportLayerProtocol, TcpProtocol>().WithName("TransportLayer");
            Register<ITransportLayerProtocol, UdpProtocol>().WithName("TransportLayer");
        }
    }

    internal class ConfigurationWithDuplicateImplementationsAndNames : WhooshConfiguration
    {
        public ConfigurationWithDuplicateImplementationsAndNames()
        {
            Register<ITransportLayerProtocol, TcpProtocol>();
            Register<ITransportLayerProtocol, TcpProtocol>();
            
            Register<ICar, RenaultCar>().WithName("Car");
            Register<ICar, VolkswagenCar>().WithName("Car");
        }
    }
}