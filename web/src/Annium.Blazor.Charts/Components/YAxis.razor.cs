using System;
using System.Globalization;
using System.Threading.Tasks;
using Annium.Blazor.Charts.Domain.Contexts;
using Annium.Blazor.Charts.Internal.Extensions;
using Annium.Blazor.Interop;
using Annium.Logging;
using Microsoft.AspNetCore.Components;
using static Annium.Blazor.Charts.Internal.Constants;

namespace Annium.Blazor.Charts.Components;

public partial class YAxis : ILogSubject, IAsyncDisposable
{
    [Parameter]
    public string LabelFontFamily { get; set; } = AxisLabelFontFamily;

    [Parameter]
    public int LabelFontSize { get; set; } = AxisLabelFontSize;

    [Parameter]
    public string LabelStyle { get; set; } = AxisLabelStyle;

    [CascadingParameter]
    public IChartContext ChartContext { get; set; } = null!;

    [CascadingParameter]
    public IPaneContext PaneContext { get; set; } = null!;

    [CascadingParameter]
    public IVerticalSideContext SideContext { get; set; } = null!;

    [Inject]
    public ILogger Logger { get; set; } = null!;

    private AsyncDisposableBox _disposable = Disposable.AsyncBox(VoidLogger.Instance);

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        _disposable += ChartContext.OnUpdate(Draw);
    }

    private void Draw()
    {
        var ctx = SideContext.Canvas;

        ctx.Save();

        ctx.Font = $"{LabelFontSize}px {LabelFontFamily}";
        ctx.FillStyle = LabelStyle;
        ctx.TextBaseline = CanvasTextBaseline.middle;

        var x = 3;
        foreach (var (y, m) in PaneContext.GetHorizontalLines())
        {
            var text = m.ToString(CultureInfo.InvariantCulture);

            ctx.FillText(text, x, y);
        }

        ctx.Restore();
    }

    public ValueTask DisposeAsync()
    {
        return _disposable.DisposeAsync();
    }
}
