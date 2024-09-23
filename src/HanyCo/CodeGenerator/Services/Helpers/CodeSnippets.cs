using System.Net.Http;                // HttpClient
using System.Net.Http.Json;           // GetFromJsonAsync, PostAsJsonAsync
using Microsoft.AspNetCore.Components; // برای اجزای Blazor

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.SqlServer;
using Library.Helpers.CodeGen;

using System.Collections.Immutable;
using System.Net.Http;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Services.Helpers;

//[DebuggerStepThrough]
internal static class CodeSnippets
{
    public static string QueryHandler_Handle_Body(in CqrsViewModelBase model, in DtoViewModel entityModel, in string? additionalWhereClause = null)
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

    internal static string BlazorDetailsComponent_LoadPage_Body(in CqrsViewModelBase cqrsViewModel) =>
        new StringBuilder()
            .AppendLine("if (this.EntityId is { } entityId)")
            .AppendLine("{")
            
            .AppendLine("}")
            .AppendLine("else")
            .AppendLine("{")
            .AppendLine("this.DataContext = new();")
            .AppendLine("}")
            .ToString();

    internal static string BlazorDetailsComponent_SaveButton_OnClick_Body(in CqrsCommandViewModel insert, in CqrsCommandViewModel update) =>
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

    internal static string BlazorListComponent_DeleteButton_OnClick_Body(in CqrsCommandViewModel model)
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

    internal static string BlazorListComponent_LoadPage_Body(in DtoViewModel resultDto, in DtoViewModel sourceDto, in string httClientInstanceName = "_http")
    {
        var result = new StringBuilder()
            .AppendLine($"""var apiResult = await {httClientInstanceName}.GetFromJsonAsync<{resultDto.Name}>("{sourceDto.Name}");""")
            .AppendLine($"""this.DataContext = apiResult;""");
        return result.ToString();
    }

    internal static string DeleteCommandHandler_Handle_Body(in CqrsCommandViewModel model, in DtoViewModel entityModel)
    {
        var additionalWhereClause = "[Id] = {request.Id}";
        var deleteStatement = SqlStatementBuilder
            .Delete(entityModel.DbObject.ToString())
            .Where(ReplaceVariables(model.ParamsDto, additionalWhereClause, $"request.{model.DbObject.Name}"))
            .Build()
            .Replace(Environment.NewLine, " ").Replace("  ", " ");
        return new StringBuilder()
            .AppendLine($"var dbCommand = $@\"{deleteStatement}\";")
            .AppendLine($"await this._sql.ExecuteNonQueryAsync(dbCommand, cancellationToken: cancellationToken);")
            .AppendLine($"var result = new {model.GetSegregateResultType("Command")}();")
            .AppendLine($"return result;")
            .ToString();
    }

    internal static string InsertCommandHandler_Handle_Body(in CqrsViewModelBase model, in DtoViewModel entityModel)
    {
        var values = GetValues(entityModel.Properties.ExcludeId(), model.DbObject.Name!).ToImmutableArray();
        var insertStatement = SqlStatementBuilder
            .Insert()
            .Into(entityModel.DbObject.ToString())
            .Values(values.Select(x => (x.ColumnName, (object)$"{{{x.VariableName}}}"))!)
            .ReturnId()
            .ForceFormatValues(false)
            .Build().Replace(Environment.NewLine, " ").Replace("  ", " ");
        var result = new StringBuilder()
            .AppendAllLines(values, x => $"var {x.VariableName} = {x.VariableStatement};")
            .AppendLine($"var dbCommand = $@\"{insertStatement}\";")
            .AppendLine($"var dbResult = await this._sql.ExecuteScalarCommandAsync(dbCommand, cancellationToken);")
            .AppendLine($"int returnValue = Convert.ToInt32(dbResult);")
            .AppendLine($"var result = new {model.GetSegregateResultType("Command").Name}(returnValue);")
            .AppendLine($"return result;")
            .ToString();
        return result;
    }

    internal static string InsertCommandValidator_Handle_Body(in CqrsCommandViewModel model, in DtoViewModel entityModel)
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

    internal static string NavigateTo(in string url) =>
        $"this._navigationManager.NavigateTo({url});";

    internal static string UpdateCommandHandler_Handle_Body(in CqrsCommandViewModel model, in DtoViewModel entityModel)
    {
        var values = GetValues(entityModel.Properties.ExcludeId(), model.DbObject.Name!).ToImmutableArray();
        var updateStatement = SqlStatementBuilder
            .Update(entityModel.DbObject.ToString())
            .Set(values.Select(x => (x.ColumnName, (object)$"{{{x.VariableName}}}")))
            .Where("[Id] = {request.Id}")
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

    internal static string UpdateCommandValidator_Handle_Body(in CqrsCommandViewModel model) =>
        new StringBuilder("_ = command.ArgumentNotNull().Params.Check()")
            .AppendLine(".RuleFor(x => x.Id > 0, () => \"Id cannot be null, zero or less than zero.\")")
            .AppendAllLines(model.ParamsDto.Properties.ExcludeId().Where(x => !(x.IsNullable ?? true)).Select(x => $".NotNull(x => x.{x.Name})"))
            .AppendLine(".ThrowOnFail();")
            .AppendLine()
            .AppendLine("return ValueTask.CompletedTask;")
            .ToString();

    private static IEnumerable<(string ColumnName, object VariableName, string VariableStatement)> GetValues(IEnumerable<PropertyViewModel> properties, string requestParamName)
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
                or PropertyType.Byte => numericColumn(dbColumn, requestParamName),
                PropertyType.Boolean => commonColumn(dbColumn, requestParamName),
                PropertyType.DateTime => dateColumn(dbColumn, requestParamName),
                _ => commonColumn(dbColumn, requestParamName),
            };
            yield return (dbColumn.Name, TypeMemberNameHelper.ToArgName(dbColumn.Name), statement);
        }

        static string commonColumn(DbColumnViewModel dbColumn, string requestParamName) =>
            dbColumn.IsNullable
                ? $"request.{requestParamName}.{dbColumn.Name}?.ToString().IsNullOrEmpty() ?? true ? \"null\" : $\"N'{{request.{requestParamName}.{dbColumn.Name}.ToString()}}'\""
                : $"$\"N'{{request.{requestParamName}.{dbColumn.Name}.ToString()}}'\"";
        static string dateColumn(DbColumnViewModel dbColumn, string requestParamName) =>
            dbColumn.IsNullable
                ? $"request.{requestParamName}.{dbColumn.Name}?.ToString().IsNullOrEmpty() ?? true ? \"null\" : $\"N{{SqlTypeHelper.FormatDate(request.{requestParamName}.{dbColumn.Name})}}\";"
                : $"$\"N{{SqlTypeHelper.FormatDate(request.{requestParamName}.{dbColumn.Name})}}\"";
        static string numericColumn(DbColumnViewModel dbColumn, string requestParamName) =>
            dbColumn.IsNullable
                ? $"request.{requestParamName}.{dbColumn.Name}?.ToString() ?? \"null\""
                : $"request.{requestParamName}.{dbColumn.Name}.ToString()";
    }

    private static string? ReplaceVariables(in DtoViewModel paramsDto, in string? statement, in string paramName)
    {
        if (statement.IsNullOrEmpty())
        {
            return string.Empty;
        }

        var result = statement;
        foreach (var p in paramsDto.Properties)
        {
            result = result
                .Replace($"%{p!.DbObject?.Name}%", $"{{{paramName}.{p.Name}}}")
                .Replace($"^{p!.DbObject?.Name}^", p.Name);
        }
        return result;
    }

    private static string GenerateApiCallCode(
        in string controllerName,
        in HttpMethod method = HttpMethod.Get,
        in string returnStatement = "var apiResult",
        in string httpClientInstanceName = "_http",
        in string? nameSpace = null,
        in IEnumerable<string>? queryParams = null,
        in IEnumerable<(string Key, string Value)>? keyValueQueryParams = null,
        in string? paramVarName = null,
        in string? resultTypeName = null
        )
    {
        string httpClientMethodName = (method) switch
        {
            HttpMethod.Get => nameof(HttpClientJsonExtensions.GetFromJsonAsync),
            HttpMethod.Post => nameof(HttpClientJsonExtensions.PostAsJsonAsync),
            HttpMethod.Put => nameof(HttpClientJsonExtensions.PutAsJsonAsync),
            HttpMethod.Delete => nameof(HttpClientJsonExtensions.DeleteFromJsonAsync),
            _ => throw new NotImplementedException()
        };
        
        // Example:
        // person = await Http.GetFromJsonAsync<PersonDto>($"http://localhost:1544/mes/HumanResources/person/{personId}");
        // var response = await Http.PostAsJsonAsync("http://localhost:1544/mes/HumanResources/person", newPerson);

        // var apiResult = await _http.GetFromJsonAsync
        var sb = new StringBuilder($"{returnStatement} = await {httpClientInstanceName.Trim('.')}.{httpClientMethodName}")
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>
            .Append(resultTypeName!.TrimStart('<').TrimEnd('>') is { } value?$"<{value}>":"")
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"""
            .Append("($\"\"\"")
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"""humanresources/
            .Append(nameSpace?.Trim('/') is { } ns ? $"{ns}/" : null)
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"""humanresources/person/
            .Append($"{controllerName.Trim('/')}/")
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"""humanresources/person/123456/
            .AppendAll(queryParams.Compact().Select(qp => $"{qp.Trim('/')}/"))
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"humanresources/person/123456/name=ali&age=5
            .Append(keyValueQueryParams.Compact(kv => kv is { Key: not null } and { Value: not null }).Select(kv => $"{kv.Key}={kv.Value}").Merge('&'))
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"""humanresources/person/123456/name=ali&age=5"""
            .Append("\"\"\"\"")
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"""humanresources/person/123456/name=ali&age=5""", newPerson
            .Append(paramVarName.IsNullOrEmpty()?null:$", {paramVarName}")
            // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"""humanresources/person/123456/name=ali&age=5""", newPerson)
            .Append(')');
        return sb.ToString();
    }

    enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete
    }
}