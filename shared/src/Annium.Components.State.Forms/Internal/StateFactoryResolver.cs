using System;
using System.Linq;
using System.Reflection;
using Annium.Reflection;

namespace Annium.Components.State.Forms.Internal;

internal static class StateFactoryResolver
{
    private static readonly MethodInfo _atomicFactory = GetFactory(nameof(IStateFactory.CreateAtomic));
    private static readonly MethodInfo _arrayFactory = GetFactory(nameof(IStateFactory.CreateArray));
    private static readonly MethodInfo _mapFactory = GetFactory(nameof(IStateFactory.CreateMap));
    private static readonly MethodInfo _objectFactory = GetFactory(nameof(IStateFactory.CreateObject));

    public static MethodInfo ResolveFactory(Type type)
    {
        if (type.IsRecordLike())
            return ResolveFactory(_mapFactory, type);

        if (type.IsArrayLike())
            return ResolveFactory(_arrayFactory, type);

        if (type.GetInterfaces().Contains(typeof(IEquatable<>).MakeGenericType(type)))
            return ResolveFactory(_atomicFactory, type);

        if (type.IsEnum)
            return ResolveFactory(_atomicFactory, type);

        return ResolveFactory(_objectFactory, type);
    }

    private static MethodInfo ResolveFactory(MethodInfo factory, Type type)
    {
        var template = factory.GetParameters().Single().ParameterType;
        if (template.IsGenericParameter)
            return factory.MakeGenericMethod(type);

        var args = template.ResolveGenericArgumentsByImplementation(type);

        if (args is null)
            throw new InvalidOperationException($"Failed to resolve state factory for {type}");

        var parameters = factory.GetGenericArguments();
        for (var i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var arg = args[i];
            if (param.ResolveByImplementation(arg) is null)
                throw new InvalidOperationException($"Failed to resolve state factory for {type}");
        }

        return factory.MakeGenericMethod(args);
    }

    private static MethodInfo GetFactory(string name) => typeof(IStateFactory).GetMethods().Single(x => x.Name == name);
}
