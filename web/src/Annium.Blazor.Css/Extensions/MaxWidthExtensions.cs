using static System.FormattableString;

// ReSharper disable once CheckNamespace
namespace Annium.Blazor.Css;

public static class MaxWidthExtensions
{
    public static CssRule MaxWidth(this CssRule rule, string maxWidth) => rule.Set("max-width", maxWidth);

    public static CssRule MaxWidthPx(this CssRule rule, int maxWidth) => rule.MaxWidth(Invariant($"{maxWidth}px"));

    public static CssRule MaxWidthEm(this CssRule rule, int maxWidth) => rule.MaxWidth(Invariant($"{maxWidth}em"));

    public static CssRule MaxWidthRem(this CssRule rule, int maxWidth) => rule.MaxWidth(Invariant($"{maxWidth}rem"));

    public static CssRule MaxWidthPercent(this CssRule rule, int maxWidth) => rule.MaxWidth(Invariant($"{maxWidth}%"));
}
