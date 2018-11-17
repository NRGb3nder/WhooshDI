using System;
using System.Collections.Generic;
using WhooshDI.Attributes;

namespace WhooshDI.UnitTests.Helpers
{
    internal class ClassWithParameterlessCtor
    {
    }

    internal class ClassWithParameterizedCtor
    {
        public ClassWithParameterlessCtor ClassWithParameterlessCtor { get; }
        
        public ClassWithParameterizedCtor(ClassWithParameterlessCtor obj) => ClassWithParameterlessCtor = obj;
    }

    internal class DisposableClass : IDisposable
    {
        public bool IsDisposed { get; private set; }
        
        public void Dispose()
        {
            IsDisposed = true;
        }
    }
    
    internal enum Cars
    {
        Renault,
        Volkswagen
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
    
    internal class CarService
    {
        public ICar Car { get; }
        
        public CarService([DependencyKey(Cars.Volkswagen)] ICar car) => Car = car;
    }

    internal class CarService<T> where T : ICar
    {
        public T Car { get; }

        public CarService(T car) => Car = car;
    }

    internal class TaxiDriver
    {
        [WhooshResolve]
        public ICar Car { get; set; }
    }

    internal class IncorrectlyDefinedTaxiDriver
    {
        [WhooshResolve]
        public ICar Car { get; }
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

    internal interface IService<T>
    {
        T Repository { get; set; }
    }

    internal class ServiceImplementation<T> : IService<T>
    {
        public T Repository { get; set; }
    }

    internal class SqlServerRepository
    {
    }

    internal class SessionLayerService
    {
        public IEnumerable<ITransportLayerProtocol> TransportLayerProtocols { get; }

        public SessionLayerService(IEnumerable<ITransportLayerProtocol> protocols)
        {
            TransportLayerProtocols = protocols;
        }
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
}