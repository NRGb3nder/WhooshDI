using FluentAssertions;
using NUnit.Framework;
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

            var instance = whoosh.Resolve<ParamlessCtorClass>();

            instance.Should().BeOfType<ParamlessCtorClass>();
        }

        [Test]
        public void CreatesAnInstanceOfAClassWithParameterizedConstructor()
        {
            var whoosh = new Whoosh();

            var instance = whoosh.Resolve<ParameterizedCtorClass>();

            instance.ParamlessCtorClass.Should().BeOfType<ParamlessCtorClass>();
        }
        
        [Test]
        public void AllowsTransientDependencies()
        {
            var config = new TransientImplConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<ParamlessCtorClass>();
            var anotherInstance = whoosh.Resolve<ParamlessCtorClass>();

            instance.Should().NotBe(anotherInstance);
        }

        [Test]
        public void DependenciesAreTransientByDefault()
        {
            var whoosh = new Whoosh();
    
            var instance = whoosh.Resolve<ParamlessCtorClass>();
            var anotherInstance = whoosh.Resolve<ParamlessCtorClass>();

            instance.Should().NotBe(anotherInstance);
        }
        
        [Test]
        public void AllowsSingletonDependencies()
        {
            var config = new SingletonImplConfig();
            var whoosh = new Whoosh(config);

            var instance = whoosh.Resolve<ParamlessCtorClass>();
            var anotherInstance = whoosh.Resolve<ParamlessCtorClass>();

            instance.Should().Be(anotherInstance);
        }
    }
}