using System;
using System.Threading.Tasks;
using Annium.Blazor.Charts.Domain.Contexts;
using Annium.Blazor.Charts.Internal.Domain.Interfaces.Contexts;
using Annium.Blazor.Core.Tools;
using Annium.Blazor.Css;
using Annium.Logging;
using Microsoft.AspNetCore.Components;

namespace Annium.Blazor.Charts.Components;

public partial class Pane : ILogSubject, IAsyncDisposable
{
    [Parameter]
    public string? CssClass { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    [CascadingParameter]
    internal IChartContext ChartContext { get; set; } = null!;

    [Inject]
    private IManagedPaneContext PaneContext { get; set; } = null!;

    [Inject]
    private Style Styles { get; set; } = null!;

    [Inject]
    public ILogger Logger { get; set; } = null!;

    private string Class => ClassBuilder.With(Styles.Block).With(CssClass).Build();
    private AsyncDisposableBox _disposable = Disposable.AsyncBox(VoidLogger.Instance);

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        PaneContext.Init(ChartContext);
        _disposable += ChartContext.RegisterPane(PaneContext);
    }

    public ValueTask DisposeAsync()
    {
        return _disposable.DisposeAsync();
    }

    public class Style : RuleSet
    {
        public readonly CssRule Block = Rule.Class()
            .WidthPercent(100)
            .Set("display", "grid")
            .Set("grid-template-rows", "1fr min-content")
            .Set("grid-template-columns", "1fr min-content");
    }
}
