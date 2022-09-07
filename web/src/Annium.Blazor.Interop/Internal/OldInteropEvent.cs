using System;

namespace Annium.Blazor.Interop.Internal;

internal sealed record OldInteropEvent<T>
{
    public event Action<T> Event = delegate { };
    public int? CallbackId { get; private set; }
    public bool HasListeners => Event.GetInvocationList().Length > 1;
    public void Handle(T x) => Event(x);

    public void SetCallbackId(int cid)
    {
        if (CallbackId.HasValue)
            throw new InvalidOperationException("Callback id is already set");

        CallbackId = cid;
    }

    public void ResetCallbackId()
    {
        if (!CallbackId.HasValue)
            throw new InvalidOperationException("Callback id is already reset");

        CallbackId = null;
    }
}