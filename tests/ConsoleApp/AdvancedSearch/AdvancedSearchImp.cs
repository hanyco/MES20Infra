using System.Text;

using Library.CodeGeneration;
using Library.Data.SqlServer;
using Library.Helpers;
using Library.Interfaces;

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
            _ = result.Append($"{SqlStatementBuilder.AddBrackets(operation.Field.Name)}");
            var operand = operation.Operator switch
            {
                AdvancedSearchFieldOperator.IsBiggerThan => $" > {operation.Parameters.First()}",
                AdvancedSearchFieldOperator.IsLessThan => $" < {operation.Parameters.First()}",
                AdvancedSearchFieldOperator.Contains => $" IS LIKE('%{operation.Parameters.First()}%')",
                AdvancedSearchFieldOperator.StartsWith => $" IS LIKE('{operation.Parameters.First()}%')",
                AdvancedSearchFieldOperator.EndsWith => $" IS LIKE('%{operation.Parameters.First()}')",
                AdvancedSearchFieldOperator.Equals => $" == {formatOperand(operation, operation.Parameters.First())}",
                AdvancedSearchFieldOperator.NotEquals => $" <> {formatOperand(operation, operation.Parameters.First())}",
                AdvancedSearchFieldOperator.IsNull => " IS NULL",
                AdvancedSearchFieldOperator.IsNotNull => " IS NOT NULL",
                _ => throw new NotImplementedException(),
            };
            _ = result.Append(operand);
        }
        return result.ToString();
        static string formatOperand(AdvancedSearchOperation operation, object? operand)
        {
            if (operation.Field.Type == typeof(int) || operation.Field.Type == typeof(long))
            {
                return operand.ToString()!;
            }

            if (operation.Field.Type == typeof(DateTime) || operation.Field.Type == typeof(DateOnly) || operation.Field.Type == typeof(TimeOnly))
            {
                return SqlTypeHelper.FormatDate(operand);
            }
            if (ObjectHelper.IsDbNull(operand))
            {
                return "null";
            };
            return $"'{operand}'";
        }
    }
}

public readonly record struct AdvancedSearchField(string Name, TypePath Type);

public readonly record struct AdvancedSearchOperation(AdvancedSearchField Field, AdvancedSearchFieldOperator Operator, IEnumerable<object?>? Parameters = null);

public enum AdvancedSearchFieldOperator
{
    IsBiggerThan,
    IsLessThan,
    Contains,
    StartsWith,
    EndsWith,
    Equals,
    NotEquals,
    IsNull,
    IsNotNull,
}

public sealed class AdvancedSearchViewModel : INew<AdvancedSearchViewModel>
{
    private AdvancedSearchViewModel()
    {
    }

    public IEnumerable<AdvancedSearchOperation> Operations => this._operations;
    private HashSet<AdvancedSearchOperation> _operations { get; } = [];

    public static AdvancedSearchViewModel New() =>
        new();

    public AdvancedSearchViewModel AddOperation(AdvancedSearchOperation operation)
    {
        _ = this._operations.Add(operation);
        return this;
    }
}