using System;
using System.Threading;
using Annium.Blazor.Charts.Internal.Domain.Interfaces.Contexts;
using Annium.Blazor.Interop;

namespace Annium.Blazor.Charts.Internal.Domain.Models.Contexts;

internal sealed record SeriesContext : IManagedSeriesContext
{
    public Canvas Canvas { get; private set; } = null!;
    public Canvas Overlay { get; private set; } = null!;
    public DomRect Rect { get; private set; }
    private int _isInitiated;

    public void Init(Canvas canvas, Canvas overlay)
    {
        if (Interlocked.CompareExchange(ref _isInitiated, 1, 0) != 0)
            throw new InvalidOperationException($"Can't init {nameof(VerticalSideContext)} more than once");

        Canvas = canvas;
        Overlay = overlay;
    }

    public void SetRect(DomRect rect)
    {
        Rect = rect;
    }
}
