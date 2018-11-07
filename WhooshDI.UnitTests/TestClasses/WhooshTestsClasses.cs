using WhooshDI.Attributes;

namespace WhooshDI.UnitTests.TestClasses
{
    internal class ClassWithParameterlessCtor
    {
    }

    internal class ClassWithParameterizedCtor
    {
        public ClassWithParameterlessCtor ClassWithParameterlessCtor { get; }
        
        public ClassWithParameterizedCtor(ClassWithParameterlessCtor obj) => ClassWithParameterlessCtor = obj;
    }
    
    internal enum Cars
    {
        Renault,
        Volkswagen
    }

    internal class CarService
    {
        public ICar Car { get; }
        
        public CarService([DependencyKey(Cars.Volkswagen)] ICar car) => Car = car;
    }

    internal interface ICar
    {
        string ModelName { get; set; }
    }

    internal class RenaultCar : ICar
    {
        public string ModelName { get; set; } = "Logan";
    }

    internal class VolkswagenCar : ICar
    {
        public string ModelName { get; set; } = "Polo";
    }

    internal interface ITransportLayerProtocol
    {
    }

    internal class TcpProtocol : ITransportLayerProtocol
    {
    }
    
    internal class UdpProtocol : ITransportLayerProtocol
    {
    }

    internal class ClassWithCircularDependency
    {
        private ClassWithDeepCircularDependency Obj { get; }

        public ClassWithCircularDependency(ClassWithDeepCircularDependency obj) => Obj = obj;
    }

    internal class ClassWithDeepCircularDependency
    {
        private ClassWithCircularDependency Obj { get; }
        
        public ClassWithDeepCircularDependency(ClassWithCircularDependency obj) => Obj = obj;
    }
    
    internal class TransientImplConfig : WhooshConfiguration
    {
        public TransientImplConfig() => Register<ClassWithParameterlessCtor>().AsTransient();
    }
    
    internal class SingletonImplConfig : WhooshConfiguration
    {
        public SingletonImplConfig() => Register<ClassWithParameterlessCtor>().AsSingleton();
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
}