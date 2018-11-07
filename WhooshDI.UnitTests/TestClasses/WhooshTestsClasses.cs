using WhooshDI.Attributes;

namespace WhooshDI.UnitTests.TestClasses
{
    public class ParamlessCtorClass
    {
    }

    public class ParameterizedCtorClass
    {
        public ParamlessCtorClass ParamlessCtorClass { get; }
        
        public ParameterizedCtorClass(ParamlessCtorClass obj)
        {
            ParamlessCtorClass = obj;
        }
    }
    
    public enum Cars 
    {
        Renault,
        Volkswagen
    }

    public class CarService
    {
        public ICar Car { get; }
        
        public CarService([DependencyKey(Cars.Volkswagen)] ICar car)
        {
            Car = car;
        }
    }

    public interface ICar
    {
        string ModelName { get; set; }
    }

    public class RenaultCar : ICar
    {
        public string ModelName { get; set; } = "Logan";
    }

    public class VolkswagenCar : ICar
    {
        public string ModelName { get; set; } = "Polo";
    }
    
    public class TransientImplConfig : WhooshConfiguration
    {
        public TransientImplConfig()
        {
            Register<ParamlessCtorClass>().AsTransient();
        }
    }
    
    public class SingletonImplConfig : WhooshConfiguration
    {
        public SingletonImplConfig()
        {
            Register<ParamlessCtorClass>().AsSingleton();
        }
    }

    public class NamedDependenciesConfig : WhooshConfiguration
    {
        public NamedDependenciesConfig()
        {
            Register<ICar, RenaultCar>().WithName(Cars.Renault);
            Register<ICar, VolkswagenCar>().WithName(Cars.Volkswagen);
        }
    }
}