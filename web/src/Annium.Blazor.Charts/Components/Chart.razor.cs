using System;
using System.Threading.Tasks;
using Annium.Blazor.Charts.Domain.Contexts;
using Annium.Blazor.Charts.Domain.Lookup;
using Annium.Blazor.Charts.Extensions;
using Annium.Blazor.Charts.Internal.Domain.Interfaces.Contexts;
using Annium.Blazor.Charts.Internal.Extensions;
using Annium.Blazor.Core.Tools;
using Annium.Blazor.Css;
using Annium.Blazor.Interop;
using Annium.Extensions.Jobs;
using Annium.Logging;
using Microsoft.AspNetCore.Components;
using static Annium.Blazor.Charts.Internal.Constants;

namespace Annium.Blazor.Charts.Components;

public partial class Chart : ILogSubject, IAsyncDisposable
{
    [Parameter, EditorRequired]
    public IChartContext ChartContext { get; set; } = null!;

    [Parameter]
    public string? CssClass { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    [Inject]
    public ILogger Logger { get; set; } = null!;

    [Inject]
    private Style Styles { get; set; } = null!;

    private string Class => ClassBuilder.With(Styles.Container).With(CssClass).Build();
    private Div _container = null!;
    private IManagedChartContext _chartContext = null!;
    private AsyncDisposableBox _disposable = Disposable.AsyncBox(VoidLogger.Instance);

    protected override void OnParametersSet()
    {
        this.Trace("request draw");
        _chartContext = (IManagedChartContext)ChartContext;
        _chartContext.RequestDraw();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        _chartContext.SetRect(_container.GetBoundingClientRect());

        _disposable += Timer.Start(CheckState, AnimationFrameMs, AnimationFrameMs);
        _disposable += _container;
        _disposable += Window.OnResize(_ => _chartContext.SetRect(_container.GetBoundingClientRect()));
        _disposable += _container.OnResize(_ => _chartContext.SetRect(_container.GetBoundingClientRect()));
        _disposable += Window.OnKeyDown(HandleKeyboard, false);
        _container.OnWheel(HandleWheel);
        _container.OnMouseMove(HandlePointerMove);
        _container.OnMouseOut(HandlePointerOut);
    }

    private void HandleKeyboard(KeyboardEvent e)
    {
        if (e is { AltKey: true } && (e.Key == "-" || e.Code == "Minus"))
            ChangeZoom(-1);

        if (e is { AltKey: true } && (e.Key == "=" || e.Code == "Equal"))
            ChangeZoom(1);
    }

    private void HandleWheel(WheelEvent e)
    {
        if (_chartContext.IsLocked)
            return;

        var changed = e.CtrlKey ? HandleZoomDelta(e.DeltaY) : HandleScrollDelta(e.DeltaX);

        if (changed)
            _chartContext.RequestDraw();
    }

    private void HandlePointerMove(MouseEvent e) => _chartContext.RequestOverlay(new Point(e.X, e.Y));

    private void HandlePointerOut(MouseEvent _) => _chartContext.RequestOverlay(null);

    private void CheckState()
    {
        if (_chartContext.TryDraw())
            _chartContext.Update();

        if (_chartContext.TryOverlay(out var point))
        {
            _chartContext.ClearOverlays();

            if (point == default)
                _chartContext.SetLookup(null, null);
            else
                _chartContext.SetLookup(_chartContext.FromX(point.X), point);
        }
    }

    private bool HandleZoomDelta(decimal delta)
    {
        return ChangeZoom(delta < 0 ? 1 : -1);
    }

    private bool ChangeZoom(int delta)
    {
        var zoomIndex = _chartContext.ResolveZoomIndex();

        var newZoomIndex = (zoomIndex + delta).Within(0, _chartContext.Zooms.Count - 1);
        if (newZoomIndex == zoomIndex)
            return false;

        _chartContext.SetZoom(_chartContext.Zooms[newZoomIndex]);

        return true;
    }

    private bool HandleScrollDelta(decimal delta)
    {
        var change = (delta * ScrollMultiplier).FloorInt32();
        if (change == 0)
            return false;

        var (start, end) = _chartContext.View;
        var size = end - start;

        // block scrolling to left of chart bounds
        if (change < 0 && end - _chartContext.Bounds.Start <= size / 2)
            return false;

        // block scrolling to right of chart bounds
        if (change > 0 && _chartContext.Bounds.End - start <= size / 2)
            return false;

        var moment = _chartContext.FromX(_chartContext.ToX(_chartContext.Moment) + change);
        _chartContext.SetMoment(moment);

        return true;
    }

    public ValueTask DisposeAsync()
    {
        return _disposable.DisposeAsync();
    }

    public class Style : RuleSet
    {
        public readonly CssRule Container = Rule.Class()
            .PositionRelative()
            .FlexColumn(AlignItems.FlexStart, JustifyContent.Stretch);
    }
}
