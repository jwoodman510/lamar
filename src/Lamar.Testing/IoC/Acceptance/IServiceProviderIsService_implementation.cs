using System.Collections.Generic;
using Shouldly;
using StructureMap.Testing.Widget;
using Xunit;

namespace Lamar.Testing.IoC.Acceptance;
#if NET6_0_OR_GREATER
public class IServiceProviderIsService_implementation
{
    [Fact]
    public void explicit_checks_of_non_concrete_types()
    {
        var container = Container.For(x => { x.For<IClock>().Use<Clock>(); });

        container.IsService(typeof(IClock)).ShouldBeTrue();
        container.IsService(typeof(IWidget)).ShouldBeFalse();
    }

    [Fact]
    public void fix_for_368_auto_resolve_enumerable_concretes_and_generics()
    {
        var container = Container.For(services =>
        {
            services.Scan(s =>
            {
                s.AssemblyContainingType(GetType());
                s.WithDefaultConventions();
                s.AddAllTypesOf<IService>();
            });

            services.For(typeof(IGenericService1<>)).Use(typeof(GenericService1<>));
        });

        container.IsService(typeof(ConcreteClass)).ShouldBeFalse();
        container.IsService(typeof(IEnumerable<IService>)).ShouldBeTrue();
        container.IsService(typeof(IGenericService1<IService>)).ShouldBeTrue();
        container.IsService(typeof(IGenericService1<ConcreteClass>)).ShouldBeTrue();
    }

    [Fact]
    public void fix_for_391_dont_resolve_collections_with_not_registered_type()
    {
        var containerWithNoRegistrations = Container.Empty();

        containerWithNoRegistrations.IsService(typeof(IEnumerable<ConcreteClass>)).ShouldBeFalse();
        containerWithNoRegistrations.IsService(typeof(IReadOnlyCollection<ConcreteClass>)).ShouldBeFalse();
        containerWithNoRegistrations.IsService(typeof(IList<ConcreteClass>)).ShouldBeFalse();
        containerWithNoRegistrations.IsService(typeof(List<ConcreteClass>)).ShouldBeFalse();

        var containerWithRegistrations = Container.For(services =>
                                                      {
                                                          services.ForConcreteType<ConcreteClass>();
                                                      });

        containerWithRegistrations.IsService(typeof(IEnumerable<ConcreteClass>)).ShouldBeTrue();
        containerWithRegistrations.IsService(typeof(IReadOnlyCollection<ConcreteClass>)).ShouldBeTrue();
        containerWithRegistrations.IsService(typeof(IList<ConcreteClass>)).ShouldBeTrue();
        containerWithRegistrations.IsService(typeof(List<ConcreteClass>)).ShouldBeTrue();
    }

    public interface IService
    {
    }

    public class ServiceA : IService
    {
    }

    public class ServiceB : IService
    {
    }

    public class ConcreteClass
    {
    }

    public interface IGenericService1<T>
    {
    }

    public class GenericService1<T> : IGenericService1<T>
    {
    }
}
#endif