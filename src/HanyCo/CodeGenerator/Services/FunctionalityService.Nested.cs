using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration;
using Library.Data.SqlServer;
using Library.Helpers.CodeGen;
using Library.Results;

using Library.Validations;

using Services.Helpers;

namespace Services;

internal partial class FunctionalityService
{
    //[DebuggerStepThrough]
    private class CodeSnippets
    {
        internal static string BlazorDetailsComponent_SaveButton_OnClick_Body(CqrsCommandViewModel insert, CqrsCommandViewModel update) =>
            new StringBuilder()
                .AppendLine($"if (DataContext.Id == default)")
                .AppendLine($"{{")
                .AppendLine($"    var @params = this.DataContext.To{insert.GetSegregateParamsType("Command").Name}();")
                .AppendLine($"    var cqParams = new {insert.GetSegregateType("Command").FullPath}(@params);")
                .AppendLine($"    var cqResult = await this._commandProcessor.ExecuteAsync<{insert.GetSegregateType("Command").FullPath}, {insert.GetSegregateResultType("Command").FullPath}>(cqParams);")
                .AppendLine($"}}")
                .AppendLine($"else")
                .AppendLine($"{{")
                .AppendLine($"    var @params = this.DataContext.To{update.GetSegregateParamsType("Command").Name}();")
                .AppendLine($"    var cqParams = new {update.GetSegregateType("Command").FullPath}(@params);")
                .AppendLine($"    var cqResult = await this._commandProcessor.ExecuteAsync<{update.GetSegregateType("Command").FullPath}, {update.GetSegregateResultType("Command").FullPath}>(cqParams);")
                .AppendLine($"}}")
                .AppendLine($"MessageComponent.Show(\"Save Data\", \"Date saved.\");")
                .ToString();

        internal static string BlazorListComponent_DeleteButton_OnClick_Body(CqrsCommandViewModel model)
        {
            var result = new StringBuilder()
                .AppendLine($"// Setup segregation parameters")
                .AppendLine($"var cmdParams = new {model.GetSegregateParamsType("Command").FullPath}()")
                .AppendLine($"{{")
                .AppendLine($"    Id = id,")
                .AppendLine($"}};")
                .AppendLine($"var cmd = new {model.GetSegregateType("Command").FullPath}(cmdParams);")
                .AppendLine($"// Invoke the command handler to apply changes.")
                .AppendLine($"var cqResult = await this._commandProcessor.ExecuteAsync<{model.GetSegregateType("Command").FullPath}, {model.GetSegregateResultType("Command").FullPath}>(cmd);")
                .AppendLine($"// Now, set let UI know that the state is changed")
                .AppendLine($"await OnInitializedAsync();")
                .AppendLine($"MessageComponent.Show(\"Delete Entity\", \"Entity deleted.\");")
                .AppendLine($"this.StateHasChanged();");

            return result.ToString();
        }

        internal static string CreateDeleteCommandHandleMethodBody(CqrsCommandViewModel model, DtoViewModel entityModel)
        {
            var additionalWhereClause = "[Id] = %Id%";
            var deleteStatement = SqlStatementBuilder
                .Delete(entityModel.DbObject.ToString())
                .Where(ReplaceVariables(model.ParamsDto, additionalWhereClause, "command.Params"))
                .Build()
                .Replace(Environment.NewLine, " ").Replace("  ", " ");
            return new StringBuilder()
                .AppendLine($"var dbCommand = $@\"{deleteStatement}\";")
                .AppendLine($"await this._sql.ExecuteNonQueryAsync(dbCommand, cancellationToken: cancellationToken);")
                .AppendLine($"var result = new {model.GetSegregateResultType("Command")}();")
                .AppendLine($"return result;")
                .ToString();
        }

        internal static string CreateInsertCommandHandleMethodBody(CqrsViewModelBase model, DtoViewModel entityModel)
        {
            var values = GetValues(entityModel.Properties).ToImmutableArray();
            var insertStatement = SqlStatementBuilder
                .Insert()
                .Into(entityModel.DbObject.ToString())
                .Values(values.Select(x => (x.ColumnName, (object)$"{{{x.VariableName}}}")))
                .ReturnId()
                .ForceFormatValues(false)
                .Build().Replace(Environment.NewLine, " ").Replace("  ", " ");
            var result = new StringBuilder()
                .AppendAllLines(values, x => $"var {x.VariableName} = {x.VariableStatement};")
                .AppendLine($"var dbCommand = $@\"{insertStatement}\";")
                .AppendLine($"var dbResult = await this._sql.ExecuteScalarCommandAsync(dbCommand, cancellationToken);")
                .AppendLine($"int id = Convert.ToInt32(dbResult);")
                .AppendLine($"var result = new {model.GetSegregateResultType("Command").Name}(id);")
                .AppendLine($"return result;")
                .ToString();
            return result;
        }

        internal static string CreateInsertCommandValidatorMethodBody(CqrsCommandViewModel model, DtoViewModel entityModel)
        {
            var checks = entityModel.Properties.ExcludeId().Where(x => !(x.IsNullable ?? true)).Select(x => $".NotNull(x => x.{x.Name})").ToImmutableArray();
            return !checks.Any()
                ? string.Empty
                : new StringBuilder("_ = command.ArgumentNotNull().Params.Check()")
                    .AppendAllLines(checks)
                    .AppendLine(".ThrowOnFail();")
                    .AppendLine()
                    .AppendLine("return ValueTask.CompletedTask;")
                    .ToString();
        }

        internal static string CreateUpdateCommandHandleMethodBody(CqrsCommandViewModel model, DtoViewModel entityModel)
        {
            var values = GetValues(entityModel.Properties.ExcludeId()).ToImmutableArray();
            var updateStatement = SqlStatementBuilder
                .Update(entityModel.DbObject.ToString())
                .Set(values.Select(x => (x.ColumnName, (object)$"{{{x.VariableName}}}")))
                .Where(ReplaceVariables(entityModel, "[Id] = %Id%", "command.Params"))
                .ForceFormatValues(false)
                .Build().Replace(Environment.NewLine, " ").Replace("  ", " ");
            var result = new StringBuilder()
                .AppendAllLines(values, x => $"var {x.VariableName} = {x.VariableStatement};")
                .AppendLine($"var dbCommand = $@\"{updateStatement}\";")
                .AppendLine("var dbResult = await this._sql.ExecuteScalarCommandAsync(dbCommand, cancellationToken);")
                .AppendLine($"var result = new {model.GetSegregateResultType("Command").Name}();")
                .AppendLine("return result;")
                .ToString();
            return result;
        }

        internal static string CreateUpdateCommandValidatorMethodBody(CqrsCommandViewModel model) =>
            new StringBuilder("_ = command.ArgumentNotNull().Params.Check()")
                .AppendLine(".RuleFor(x => x.Id > 0, () => \"Id cannot be null, zero or less than zero.\")")
                .AppendAllLines(model.ParamsDto.Properties.ExcludeId().Where(x => !(x.IsNullable ?? true)).Select(x => $".NotNull(x => x.{x.Name})"))
                .AppendLine(".ThrowOnFail();")
                .AppendLine()
                .AppendLine("return ValueTask.CompletedTask;")
                .ToString();

        internal static string GetById_LoadMethodBody(CqrsViewModelBase cqrsViewModel) =>
            new StringBuilder()
                .AppendLine("if (this.EntityId is { } entityId)")
                .AppendLine("{")
                .AppendLine($"// Setup segregation parameters")
                .AppendLine($"var @params = new {cqrsViewModel.GetSegregateParamsType("Query").FullPath}()")
                .AppendLine("{")
                .AppendLine("    Id = entityId,")
                .AppendLine("};")
                .AppendLine("")
                .AppendLine($"var cqParams = new {cqrsViewModel.GetSegregateType("Query").FullPath}(@params);")
                .AppendLine($"// Invoke the query handler to retrieve all entities")
                .AppendLine($"var cqResult = await this._queryProcessor.ExecuteAsync<{cqrsViewModel.GetSegregateResultType("Query").FullPath}>(cqParams);")
                .AppendLine($"")
                .AppendLine($"")
                .AppendLine($"// Now, set the data context.")
                .AppendLine($"this.DataContext = cqResult.Result.ToViewModel();")
                .AppendLine("}")
                .AppendLine("else")
                .AppendLine("{")
                .AppendLine("this.DataContext = new();")
                .AppendLine("}")
                .ToString();

        internal static string NavigateTo(string url) =>
            $"this._navigationManager.NavigateTo({url});";

        public static string CreateQueryHandleMethodBody(CqrsViewModelBase model, DtoViewModel entityModel, string? additionalWhereClause = null)
        {
            // Create query to be used inside the body code.
            var bodyQuery = SqlStatementBuilder
                .Select(entityModel.DbObject.ToString())
                .Top(model.ResultDto.IsList ? null : 1)
                .Columns(entityModel.Properties.Select(x => x.DbObject?.Name).Compact())
                .Where(ReplaceVariables(model.ParamsDto, additionalWhereClause, "query.Params"))
                .Build()
                .Replace(Environment.NewLine, " ").Replace("  ", " ");
            // Create body code.
            (var sqlMethod, var toListMethod) = model.ResultDto.IsList
                ? (nameof(Sql.SelectAsync), $".{nameof(EnumerableHelper.ToListAsync)}(cancellationToken)")
                : (nameof(Sql.FirstOrDefaultAsync), string.Empty);
            var result = new StringBuilder()
                .AppendLine($"var dbQuery = $@\"{bodyQuery}\";")
                .AppendLine($"var dbResult = await this._sql.{sqlMethod}<{entityModel.Name}>(dbQuery){toListMethod};")
                .AppendLine($"var result = new {model.GetSegregateResultType("Query").Name}(dbResult);")
                .Append($"return result;")
                .ToString();
            return result;
        }

        private static IEnumerable<(string ColumnName, object VariableName, string VariableStatement)> GetValues(IEnumerable<PropertyViewModel> properties)
        {
            foreach (var p in properties.Compact())
            {
                var dbColumn = p.DbObject.Cast().As<DbColumnViewModel>();
                if (dbColumn is null or { Name: null or { Length: 0 } } or { DbType: null or { Length: 0 } })
                {
                    continue;
                }

                var type = PropertyTypeHelper.FromDbType(dbColumn.DbType);
                var statement = type switch
                {
                    PropertyType.Integer
                    or PropertyType.Long
                    or PropertyType.Short
                    or PropertyType.Float
                    or PropertyType.Byte => numericColumn(dbColumn),
                    PropertyType.Boolean => commonColumn(dbColumn),
                    PropertyType.DateTime => dateColumn(dbColumn),
                    _ => commonColumn(dbColumn),
                };
                yield return (dbColumn.Name, TypeMemberNameHelper.ToArgName(dbColumn.Name), statement);
            }

            static string commonColumn(DbColumnViewModel dbColumn) =>
                dbColumn.IsNullable
                    ? $"command.Params.{dbColumn.Name}?.ToString().IsNullOrEmpty() ?? true ? \"null\" : $\"N'{{command.Params.{dbColumn.Name}.ToString()}}'\""
                    : $"$\"N'{{command.Params.{dbColumn.Name}.ToString()}}'\"";
            static string dateColumn(DbColumnViewModel dbColumn) =>
                dbColumn.IsNullable
                    ? $"command.Params.{dbColumn.Name}?.ToString().IsNullOrEmpty() ?? true ? \"null\" : $\"N'{{SqlTypeHelper.FormatDate(command.Params.{dbColumn.Name})}}'\";"
                    : $"$\"N'{{SqlTypeHelper.FormatDate(command.Params.{dbColumn.Name})}}'\"";
            static string numericColumn(DbColumnViewModel dbColumn) =>
                dbColumn.IsNullable
                    ? $"command.Params.{dbColumn.Name}?.ToString() ?? \"null\""
                    : $"command.Params.{dbColumn.Name}.ToString()";
        }
    }

    private sealed class CreationData(FunctionalityViewModel result, string sourceDtoName, CancellationTokenSource tokenSource)
    {
        internal readonly string COMMENT = "Auto-generated by Functionality Service.";
        private Result<FunctionalityViewModel>? _result;

        [NotNull]
        internal CancellationTokenSource CancellationTokenSource { get; } = tokenSource.ArgumentNotNull();

        [NotNull]
        internal Result<FunctionalityViewModel> Result => this._result ??= new(this.ViewModel);

        internal string SourceDtoName { get; } = sourceDtoName ?? result.SourceDto.Name!;

        internal TypePath SourceDtoType => this.ViewModel.SourceDto.GetSourceDtoType();

        [NotNull]
        internal FunctionalityViewModel ViewModel { get; } = result.ArgumentNotNull();
    }
}