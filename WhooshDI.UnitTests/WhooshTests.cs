using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using WhooshDI.Exceptions;
using WhooshDI.UnitTests.Helpers;

namespace WhooshDI.UnitTests
{
    [TestFixture]
    public class Whoosh_Ctor_WithConfiguration
    {
        [Test]
        public void ThrowsArgumentNullExceptionWhenConfigurationIsNull()
        {
            WhooshConfiguration config = null;
            
            Action act = () => new Whoosh(config);

            act.Should().Throw<ArgumentNullException>();
        }
    }
    
    [TestFixture]
    public class Whoosh_Resolve
    {
        [Test]
        public void CreatesAnInstanceOfAClassWithParameterlessConstructor()
        {
            var whoosh = new Whoosh();

            var instance = whoosh.Resolve<ClassWithParameterlessCtor>();

            instance.Should().BeOfType<ClassWithParameterlessCtor>();
        }

        [Test]
        public void CreatesAnInstanceOfAClassWithParameterizedConstructor()
        {
            var whoosh = new Whoosh();

            var instance = whoosh.Resolve<ClassWithParameterizedCtor>();

            instance.ClassWithParameterlessCtor.Should().BeOfType<ClassWithParameterlessCtor>();
        }
        
        [Test]
        public void AllowsTransientDependencies()
        {
            var config = new TransientImplConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<ClassWithParameterlessCtor>();
            var anotherInstance = whoosh.Resolve<ClassWithParameterlessCtor>();

            instance.Should().NotBe(anotherInstance);
        }

        [Test]
        public void DependenciesAreTransientByDefault()
        {
            var whoosh = new Whoosh();
    
            var instance = whoosh.Resolve<ClassWithParameterlessCtor>();
            var anotherInstance = whoosh.Resolve<ClassWithParameterlessCtor>();

            instance.Should().NotBe(anotherInstance);
        }
        
        [Test]
        public void AllowsSingletonDependencies()
        {
            var config = new SingletonImplConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<ClassWithParameterlessCtor>();
            var anotherInstance = whoosh.Resolve<ClassWithParameterlessCtor>();

            instance.Should().Be(anotherInstance);
        }
        
        [Test]
        public void AllowsNamesInParameterAttributes()
        {
            var config = new NamedDependenciesConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<CarService>();

            instance.Car.Should().BeOfType<VolkswagenCar>();
        }
        
        [Test]
        public void AllowsGenericDependencies()
        {
            var whoosh = new Whoosh();

            var instance = whoosh.Resolve<CarService<RenaultCar>>();

            instance.Should().BeOfType<CarService<RenaultCar>>();
        }

        [Test]
        public void ResolvesAllRegisteredImplementationsInIEnumerable()
        {
            var config = new TransportProtocolsConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<IEnumerable<ITransportLayerProtocol>>();

            instance.Should().Contain(i => i.GetType() == typeof(TcpProtocol))
                .And.Subject.Should().Contain(i => i.GetType() == typeof(UdpProtocol));
        }

        [Test]
        public void ResolvesAllRegisteredImplementationsInDeepHiddenIEnumerable()
        {
            var config = new TransportProtocolsConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<SessionLayerService>();

            instance.TransportLayerProtocols.Should().Contain(i => i.GetType() == typeof(TcpProtocol))
                .And.Subject.Should().Contain(i => i.GetType() == typeof(UdpProtocol));
        }

        [Test]
        public void ThrowsCircularDependencyExceptionWhenCircularDependencyDetected()
        {
            var whoosh = new Whoosh();

            Action act = () => whoosh.Resolve<ClassWithDeepCircularDependency>();
            
            act.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void ThrowsInvalidOperationExceptionWhenDependencyCanNotBeInstantiated()
        {
            var whoosh = new Whoosh();

            Action act = () => whoosh.Resolve<ICar>();

            act.Should().Throw<InvalidOperationException>();
        }
        
        [Test]
        public void ThrowsInvalidOperationExceptionWhenResolvingUnregisteredDependencyWithName()
        {
            var config = new TransientImplConfig();
            var whoosh = new Whoosh(config);

            Action act = () => whoosh.Resolve<ITransportLayerProtocol>();
                
            act.Should().Throw<InvalidOperationException>();
        }
        
        [Test]
        public void ThrowsInvalidOperationExceptionWhenResolvingRegisteredDependencyWithUnregisteredName()
        {
            var config = new NoNamesConfig();
            var whoosh = new Whoosh(config);
            Action act = () => whoosh.Resolve<ITransportLayerProtocol>("TCP");
            act.Should().Throw<InvalidOperationException>();
        }
    }

    [TestFixture]
    public class Whoosh_Resolve_WithName
    {
        [Test]
        public void AllowsNamedDependencies()
        {
            var config = new NamedDependenciesConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<ICar>(Cars.Volkswagen);

            instance.Should().BeOfType<VolkswagenCar>();
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenNameIsNull()
        {
            var config = new NamedDependenciesConfig();
            var whoosh = new Whoosh(config);

            Action act = () => whoosh.Resolve<ICar>(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ThrowsInvalidOperationExceptionWhenConfigurationIsNotProvided()
        {
            var whoosh = new Whoosh();

            Action act = () => whoosh.Resolve<ICar>(Cars.Renault);

            act.Should().Throw<InvalidOperationException>();
        }
    }
}