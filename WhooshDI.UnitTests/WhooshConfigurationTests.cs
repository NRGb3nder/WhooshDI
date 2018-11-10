using System;
using FluentAssertions;
using NUnit.Framework;
using WhooshDI.Exceptions;
using WhooshDI.UnitTests.Helpers;

namespace WhooshDI.UnitTests
{
    [TestFixture]
    public class WhooshConfiguration_Validate
    {
        [Test]
        public void ThrowsDuplicateElementExceptionWhenHasDuplicateImplementationsForDependency()
        {
            var config = new ConfigurationWithDuplicateImplementations();

            Action act = () => config.Validate();

            act.Should().Throw<DuplicateElementException>();
        }

        [Test]
        public void ThrowsDuplicateElementExceptionWhenHasDuplicateImplementationNamesForDependency()
        {
            var config = new ConfigurationWithDuplicateNames();

            Action act = () => config.Validate();

            act.Should().Throw<DuplicateElementException>();
        }

        [Test]
        public void ThrowsAggregateExceptionWhenHasDuplicateImplementationAndNamesForDependency()
        {
            var config = new ConfigurationWithDuplicateImplementationsAndNames();

            Action act = () => config.Validate();

            act.Should().Throw<AggregateException>();
        }
    }

    [TestFixture]
    public class WhooshConfiguration_Register_NonGenericWithTwoTypeArguments
    {     
        [Test]
        public void ThrowsArgumentNullExceptionWhenDependencyTypeIsNull()
        {
            Action act = () => new ConfigurationWithNullDependencyType();

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenImplementationTypeIsNull()
        {
            Action act = () => new ConfigurationWithNullImplementationType();

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ThrowsArgumentExceptionWhenDependencyTypeIsValueType()
        {
            Action act = () => new ConfigurationWithValueTypeDependency();

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ThrowsArgumentExceptionWhenImplementationTypeIsNotAssignableToDependencyType()
        {
            Action act = () => new ConfigurationWithImplementationTypeThatIsNotAssignableToDependencyType();

            act.Should().Throw<ArgumentException>();
        }
    }
}