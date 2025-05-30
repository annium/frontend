using System.Runtime.CompilerServices;
using Annium.Blazor.Routing;
using Annium.Blazor.Routing.Internal;

[assembly: InternalsVisibleTo("Annium.Blazor.Routing.Tests")]

// ReSharper disable once CheckNamespace
namespace Annium.Core.DependencyInjection;

public static class ServiceContainerExtensions
{
    public static IServiceContainer AddRouting(this IServiceContainer container)
    {
        container.Add<RouteManager>().AsInterfaces().Singleton();
        container.Add<RouteFactory>().AsInterfaces().Singleton();
        container.AddAll().AssignableTo<IRouting>().AsInterfaces().Singleton();

        return container;
    }
}
