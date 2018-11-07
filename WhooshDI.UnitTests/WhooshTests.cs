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
    }
}