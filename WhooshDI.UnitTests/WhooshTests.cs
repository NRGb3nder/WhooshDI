using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using FluentAssertions;
using NUnit.Framework;
using WhooshDI.Configuration;
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
            var config = new TransientImplementationConfiguration();
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
            var config = new SingletonImplementationConfiguration();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<ClassWithParameterlessCtor>();
            var anotherInstance = whoosh.Resolve<ClassWithParameterlessCtor>();

            instance.Should().Be(anotherInstance);
        }
        
        [Test]
        public void AllowsNamesInParameterAttributes()
        {
            var config = new ConfigurationWithNamedDependencies();
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
        public void AllowsCollectionDependencies()
        {
            var whoosh = new Whoosh();

            var instance = whoosh.Resolve<List>();

            instance.Should().BeOfType<List>();
        }

        [Test]
        public void AllowsGenericCollectionDependencies()
        {
            var whoosh = new Whoosh();

            var instance = whoosh.Resolve<List<int>>();

            instance.Should().BeOfType<List<int>>();
        }

        [Test]
        public void AllowsConfigurationWithNonGenericRegistrations()
        {
            var config = new TaxiConfigurationThatUsesNonGenericRegistrationMethod();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<ICar>();

            instance.Should().BeOfType<RenaultCar>();
        }

        [Test]
        public void AllowsUserDefinedInstances()
        {
            var config = new ConfigurationClassWithUserDefinedInstance();
            var whoosh = new Whoosh(config);

            var firstResolvedInstance = whoosh.Resolve<ICar>();
            var secondResolveInstance = whoosh.Resolve<ICar>();

            firstResolvedInstance.Should().Be(secondResolveInstance);
        }

        [Test]
        public void ResolvesAllRegisteredImplementationsInIEnumerable()
        {
            var config = new TransportProtocolsConfiguration();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<IEnumerable<ITransportLayerProtocol>>();

            instance.Should().Contain(i => i.GetType() == typeof(TcpProtocol))
                .And.Subject.Should().Contain(i => i.GetType() == typeof(UdpProtocol));
        }

        [Test]
        public void ResolvesAllRegisteredImplementationsInDeepHiddenIEnumerable()
        {
            var config = new TransportProtocolsConfiguration();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<SessionLayerService>();

            instance.TransportLayerProtocols.Should().Contain(i => i.GetType() == typeof(TcpProtocol))
                .And.Subject.Should().Contain(i => i.GetType() == typeof(UdpProtocol));
        }

        [Test]
        public void ResolvesDependenciesInPropertiesMarkedWithWhooshResolveAttribute()
        {
            var config = new TaxiConfiguration();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<TaxiDriver>();

            instance.Car.Should().BeOfType<RenaultCar>();
        }


        [Test]
        public void ResolvesDependenciesThatAreRegisteredUsingOpenGenerics()
        {
            var config = new ConfigurationWithOpenGenericsRegistration();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<IService<SqlServerRepository>>();

            instance.Should().BeOfType<ServiceImplementation<SqlServerRepository>>();
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
            var config = new TransientImplementationConfiguration();
            var whoosh = new Whoosh(config);

            Action act = () => whoosh.Resolve<ITransportLayerProtocol>();
                
            act.Should().Throw<InvalidOperationException>();
        }
        
        [Test]
        public void ThrowsInvalidOperationExceptionWhenResolvingRegisteredDependencyWithUnregisteredName()
        {
            var config = new ConfigurationWithoutDependencyNames();
            var whoosh = new Whoosh(config);
            
            Action act = () => whoosh.Resolve<ITransportLayerProtocol>("TCP");
            
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ThrowsInvalidOperationExceptionWhenPropertyWithDependencyHasNoAccessibleSetter()
        {
            var config = new TaxiConfiguration();
            var whoosh = new Whoosh(config);

            Action act = () => whoosh.Resolve<IncorrectlyDefinedTaxiDriver>();

            act.Should().Throw<InvalidOperationException>();
        }
    }

    [TestFixture]
    public class Whoosh_Resolve_WithName
    {
        [Test]
        public void AllowsNamedDependencies()
        {
            var config = new ConfigurationWithNamedDependencies();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<ICar>(Cars.Volkswagen);

            instance.Should().BeOfType<VolkswagenCar>();
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenNameIsNull()
        {
            var config = new ConfigurationWithNamedDependencies();
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

    [TestFixture]
    public class Whoosh_BeginScope
    {
        [Test]
        public void ConstructedScopeIsNotAParentScope()
        {
            var whoosh = new Whoosh();

            var scope = whoosh.BeginScope();

            scope.Should().NotBe(whoosh);
        }
        
        [Test]
        public void ConstructedScopeResolvesDependencies()
        {
            var whoosh = new Whoosh();

            ClassWithParameterlessCtor instance;
            using (var scope = whoosh.BeginScope())
            {
                instance = scope.Resolve<ClassWithParameterlessCtor>();
            }

            instance.Should().NotBeNull();
        }

        [Test]
        public void AllowsNestedScopes()
        {
            var whoosh = new Whoosh();

            ClassWithParameterlessCtor instance;
            using (var scope = whoosh.BeginScope())
            {
                using (var nestedScope = whoosh.BeginScope())
                {
                    instance = nestedScope.Resolve<ClassWithParameterlessCtor>();
                }
            }

            instance.Should().NotBeNull();
        }

        [Test]
        public void ConstructedScopeDisposesAllResolvedDependences()
        {
            var whoosh = new Whoosh();

            DisposableClass instance;
            using (var scope = whoosh.BeginScope())
            {
                instance = scope.Resolve<DisposableClass>();
            }

            instance.IsDisposed.Should().BeTrue();
        }
    }
}