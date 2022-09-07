using System;
using Annium.Blazor.Interop.Internal.Extensions;
using Annium.Core.Primitives;
using Microsoft.JSInterop;

namespace Annium.Blazor.Interop.Internal;

internal sealed record InteropEvent<TS, TE> : IObservable<TE>
    where TS : class
{
    private static readonly IInteropContext Ctx = InteropContext.Instance;

    private bool _hasListeners;
    private int _callbackId;
    private readonly IObject _el;
    private readonly string _eventKey;
    private readonly DotNetObjectReference<TS> _objectRef;
    private readonly string _callbackName;

    public InteropEvent(IObject el,
        string eventKey,
        DotNetObjectReference<TS> objectRef,
        string callbackName
    )
    {
        _el = el;
        _eventKey = eventKey;§§
        _objectRef = objectRef;
        _callbackName = callbackName;
    }

    public IDisposable Subscribe(IObserver<TE> observer)
    {
        if (!_hasListeners)
            _callbackId = Ctx.Invoke<int>($"element.on{typeof(TE).Name}", _el.Id, _objectRef, _callbackName);

        _wheelEvent.Event += handle;

        return Disposable.Create(() =>
            {
                _wheelEvent.Event -= handle;

                if (_wheelEvent.HasListeners)
                    return;

                Ctx.InvokeVoid("element.offWheelEvent", Id, _wheelEvent.CallbackId);
                _wheelEvent.ResetCallbackId();
            }
        );
    }
}