namespace WhooshDI.UnitTests.Helpers
{
    internal class TransientImplConfig : WhooshConfiguration
    {
        public TransientImplConfig() => Register<ClassWithParameterlessCtor>().AsTransient();
    }
    
    internal class SingletonImplConfig : WhooshConfiguration
    {
        public SingletonImplConfig() => Register<ClassWithParameterlessCtor>().AsSingleton();
    }

    internal class TransportProtocolsConfig : WhooshConfiguration
    {
        public TransportProtocolsConfig()
        {
            Register<ITransportLayerProtocol, TcpProtocol>().WithName("TCP");
            Register<ITransportLayerProtocol, UdpProtocol>().WithName("UDP");
        }
    }

    internal class NamedDependenciesConfig : WhooshConfiguration
    {
        public NamedDependenciesConfig()
        {
            Register<ICar, RenaultCar>().WithName(Cars.Renault);
            Register<ICar, VolkswagenCar>().WithName(Cars.Volkswagen);
        }
    }

    internal class NoNamesConfig : WhooshConfiguration
    {
        public NoNamesConfig()
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
            Register<ITransportLayerProtocol, TcpProtocol>();
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