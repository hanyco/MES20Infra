using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.Helpers;

public static class Extensions
{
    public static CodeCategory GetCategory(this Code code)
        => code.props().Category;

    public static Code SetCategory(this Code code, CodeCategory category)
    {
        code.props().Category = category;
        return code;
    }
}