using static System.FormattableString;

// ReSharper disable once CheckNamespace
namespace Annium.Blazor.Css;

public static class FontExtensions
{
    public static CssRule FontFamily(this CssRule rule, string fontFamily) => rule.Set("font-family", fontFamily);

    public static CssRule FontSize(this CssRule rule, string fontSize) => rule.Set("font-size", fontSize);

    public static CssRule FontSizePx(this CssRule rule, double fontSize) =>
        rule.Set("font-size", Invariant($"{fontSize}px"));

    public static CssRule FontSizeEm(this CssRule rule, double fontSize) =>
        rule.Set("font-size", Invariant($"{fontSize}em"));

    public static CssRule FontSizeRem(this CssRule rule, double fontSize) =>
        rule.Set("font-size", Invariant($"{fontSize}rem"));

    public static CssRule FontWeight(this CssRule rule, FontWeight fontWeight) => rule.Set("font-weight", fontWeight);

    public static CssRule FontWeightNormal(this CssRule rule) => rule.Set("font-weight", Css.FontWeight.W400);

    public static CssRule FontWeightBold(this CssRule rule) => rule.Set("font-weight", Css.FontWeight.W700);

    public static CssRule Color(this CssRule rule, string color) => rule.Set("color", color);
}
