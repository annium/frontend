using static System.FormattableString;

// ReSharper disable once CheckNamespace
namespace Annium.Blazor.Css;

public static class MinHeightExtensions
{
    public static CssRule MinHeight(this CssRule rule, string minHeight) => rule.Set("min-height", minHeight);

    public static CssRule MinHeightPx(this CssRule rule, int minHeight) => rule.MinHeight(Invariant($"{minHeight}px"));

    public static CssRule MinHeightEm(this CssRule rule, int minHeight) => rule.MinHeight(Invariant($"{minHeight}em"));

    public static CssRule MinHeightRem(this CssRule rule, int minHeight) =>
        rule.MinHeight(Invariant($"{minHeight}rem"));

    public static CssRule MinHeightPercent(this CssRule rule, int minHeight) =>
        rule.MinHeight(Invariant($"{minHeight}%"));
}
