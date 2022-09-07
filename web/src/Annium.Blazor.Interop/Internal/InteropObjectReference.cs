using System;
using System.Collections.Concurrent;

namespace Annium.Blazor.Interop.Internal;

internal sealed record InteropObjectReference : IDisposable
{
    private readonly ConcurrentDictionary<(Type, object), object> _interopEvents = new();

    public InteropObjectReference(IObject target)
    {
    }

    public IObservable<TE> EventProxy<TE>(object key) => _interopEvents.GetOrAdd((typeof(TE), key),static(_,_));
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}