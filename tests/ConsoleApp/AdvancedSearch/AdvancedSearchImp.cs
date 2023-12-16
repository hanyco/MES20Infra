using System.Text;

using Library.CodeGeneration;

namespace ConsoleApp.AdvancedSearch;

public static class AdvancedSearchImp
{
    internal static string GenerateCode(AdvancedSearchViewModel model)
    {
        var result = new StringBuilder();
        foreach (var operation in model.Operations)
        {
            if (result.Length > 0)
            {
                _ = result.Append(" AND ");
            }
            _ = result.Append(operation.Field.Name);
            _ = result.Append(' ');
            var operand = operation.Operator switch
            {
                AdvancedSearchFieldOperator.IsBiggerThan => $" > {operation.Parameters.First()}",
                AdvancedSearchFieldOperator.IsLessThan => $" < {operation.Parameters.First()}",
                AdvancedSearchFieldOperator.Contains => $" IS LIKE('%{operation.Parameters.First()}%')",
                AdvancedSearchFieldOperator.StartsWith => $" IS LIKE('{operation.Parameters.First()}%')",
                AdvancedSearchFieldOperator.EndsWith => $" IS LIKE('%{operation.Parameters.First()}')",
                AdvancedSearchFieldOperator.Equals => $" == '{operation.Parameters.First()}'",
                AdvancedSearchFieldOperator.NotEquals => $" <> '{operation.Parameters.First()}'",
                _ => throw new NotImplementedException(),
            };
            _ = result.Append(operand);
        }
        return result.ToString();
    }
}

public readonly record struct AdvancedSearchField(string Name, TypePath Type);

public readonly record struct AdvancedSearchOperation(AdvancedSearchField Field, AdvancedSearchFieldOperator Operator, IEnumerable<object> Parameters);

public enum AdvancedSearchFieldOperator
{
    IsBiggerThan,
    IsLessThan,
    Contains,
    StartsWith,
    EndsWith,
    Equals,
    NotEquals
}

public sealed class AdvancedSearchViewModel
{
    public HashSet<AdvancedSearchOperation> Operations { get; } = [];
}