using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;
using Library.Validations;

namespace HanyCo.Infra.CodeGeneration.Helpers;

public static class CommonExtensions
{
    public static IEnumerable<IHtmlElement> GetAllChildren(this IHasChildren<IHtmlElement> element)
    {
        var el = element.ArgumentNotNull(nameof(element));
        foreach (var child in el.Children)
        {
            yield return child;
            if (child is IHasChildren<IHtmlElement> parent)
            {
                foreach (var grandChild in parent.GetAllChildren())
                {
                    yield return grandChild;
                }
            }
        }
    }
    public static IEnumerable<IHtmlElement> GetAllChildren(this IParent<IHtmlElement> element)
    {
        var el = element.ArgumentNotNull(nameof(element));
        foreach (var child in el.Children)
        {
            yield return child;
            if (child is IParent<IHtmlElement> parent)
            {
                foreach (var grandChild in parent.GetAllChildren())
                {
                    yield return grandChild;
                }
            }
        }
    }
}
