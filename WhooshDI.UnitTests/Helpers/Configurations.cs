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

    internal class TaxiConfigurationThatUsesNonGenericRegistrationMethod : WhooshConfiguration
    {
        public TaxiConfigurationThatUsesNonGenericRegistrationMethod() => Register(typeof(ICar), typeof(RenaultCar));
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

    internal class ConfigurationClassWithUserDefinedInstance : WhooshConfiguration
    {
        public ConfigurationClassWithUserDefinedInstance()
        {
            Register<ICar>(new RenaultCar());
        }
    }

    internal class ConfigurationWithOpenGenericsRegistration : WhooshConfiguration
    {
        public ConfigurationWithOpenGenericsRegistration() => 
            Register(typeof(IService<>), typeof(ServiceImplementation<>));
    }

    internal class ConfigurationWithNullDependencyType : WhooshConfiguration
    {
        public ConfigurationWithNullDependencyType() => Register(null, typeof(RenaultCar));
    }
    
    internal class ConfigurationWithNullImplementationType : WhooshConfiguration
    {
        public ConfigurationWithNullImplementationType() => Register(typeof(ICar), null);
    }

    internal class ConfigurationWithValueTypeDependency : WhooshConfiguration
    {
        public ConfigurationWithValueTypeDependency() => Register(typeof(int), typeof(int));
    }
    
    internal class ConfigurationWithImplementationTypeThatIsNotAssignableToDependencyType : WhooshConfiguration
    {
        public ConfigurationWithImplementationTypeThatIsNotAssignableToDependencyType() => 
            Register(typeof(ICar), typeof(TcpProtocol));
    }
}