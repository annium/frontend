namespace Annium.Blazor.Css;

public abstract class RuleSet
{
    protected RuleSet()
    {
        Internal.StyleSheet.Instance.Add(this);
    }
}
