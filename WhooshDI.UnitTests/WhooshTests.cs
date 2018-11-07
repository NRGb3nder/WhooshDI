using System;
using FluentAssertions;
using NUnit.Framework;
using WhooshDI.Exceptions;
using WhooshDI.UnitTests.TestClasses;

namespace WhooshDI.UnitTests
{
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
        public void ThrowsCircularDependencyExceptionWhenCircularDependencyDetected()
        {
            var whoosh = new Whoosh();

            var act = new Action(() => whoosh.Resolve<ClassWithDeepCircularDependency>());
            
            act.Should().Throw<CircularDependencyException>();
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
        public void NamesCouldBeProvidedInParameterAttributes()
        {
            var config = new NamedDependenciesConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<CarService>();

            instance.Car.Should().BeOfType<VolkswagenCar>();
        }
    }
}