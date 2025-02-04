﻿using HanyCo.Infra.CodeGen.Domain;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration;
using Library.Data.SqlServer;
using Library.Helpers.CodeGen;
using Library.Web.Results;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;

namespace Services.Helpers;

public static class CodeSnippets
{
    public static string Component_OnInitializedAsync_MethodBody(in string? onInitializedAsyncAdditionalBody)
    {
        var result = new StringBuilder(onInitializedAsyncAdditionalBody)
            .AppendLine()
            .AppendLine($"// Call developer's method.")
            .AppendLine($"await this.OnLoadAsync();");
        return result.ToString();
    }

    public static string ExecuteCqrs_MethodBody(CqrsViewModelBase cqrsViewModel) =>
        new StringBuilder()
            .AppendLine($"// Setup segregation parameters")
            .AppendLine($"var @params = new {cqrsViewModel.GetSegregateParamsType("Query").FullPath}();")
            .AppendLine($"var cqParams = new {cqrsViewModel.GetSegregateType("Query").FullPath}(@params);")
            .AppendLine($"")
            .AppendLine($"")
            .AppendLine($"// Invoke the query handler to retrieve all entities")
            .AppendLine($"var cqResult = await this._queryProcessor.ExecuteAsync<{cqrsViewModel.GetSegregateResultType("Query").FullPath}>(cqParams);")
            .AppendLine($"")
            .AppendLine($"")
            .AppendLine($"// Now, set the data context.")
            .AppendLine($"this.DataContext = cqResult.Result.ToViewModel();")
            .ToString();
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

    internal static string BlazorDetailsComponent_LoadPage_Body(in string controllerName, in string sourceDtoName)
    {
        var result = new StringBuilder()
            .AppendLine(GenerateApiCallCode(controllerName, queryParams: ["{entityId}"], resultTypeName: TypePath.New(sourceDtoName),
                startLines: [
                    "if (this.EntityId is { } entityId)",
                    "{"
                    ],
                endLines: [
                    "this.DataContext = apiResult;",
                    "}",
                    "else",
                    "{",
                    "this.DataContext = new();",
                    "}"
                    ]))
            .AppendLine("   this.DataContext = apiResult;")
            .AppendLine("}")
            .AppendLine("else")
            .AppendLine("{")
            .AppendLine("   this.DataContext = new();")
            .AppendLine("}")
            .ToString();
        return result;
    }

    internal static string BlazorDetailsComponent_SaveButton_OnClick_Body(in string controllerName, in string sourceDtoName)
    {
        var result = GenerateApiCallCode(controllerName, method: HttpMethod.Post, paramVarName: "DataContext",
                startLines: [
                    $"if (this.DataContext.Id == default)",
                    $"{{"
                    ],
                endLines: [
                    $"}}",
                    $"else",
                    $"{{",
                    GenerateApiCallCode(controllerName, method: HttpMethod.Put, queryParams: ["{this.DataContext.Id}"], paramVarName: "DataContext", embrassInToken: false),
                    $"MessageComponent.Show(\"Save Data\", \"Data saved successfully.\");",
                    $"}}",
                    ]);
        return result;
    }
        

    internal static string BlazorListComponent_DeleteButton_OnClick_Body(in string controllerName, in string sourceDtoName)
    {
        var result = GenerateApiCallCode(controllerName, method: HttpMethod.Delete, queryParams: ["{id}"], type: "typeof(bool)",
            endLines:
            [
                $"await OnInitializedAsync();",
                $@"MessageComponent.Show(""Delete Entity"", ""Entity deleted."");",
                $"this.StateHasChanged();"
            ]);

        return result.ToString();
    }

    internal static string BlazorListComponent_LoadPage_Body(in string controllerName, in string resultDtoName, in string sourceDtoName, in string httClientInstanceName = "_http")
    {
        var result = GenerateApiCallCode(controllerName, resultTypeName: TypePath.NewEnumerable(sourceDtoName), closeStatement: false,
            endLines: [".ToListAsync();", "this.DataContext = apiResult;"]);
        return result.ToString();
    }

    internal static string DeleteCommandHandler_Handle_Body(in CqrsCommandViewModel model, in DtoViewModel entityModel)
    {
        var additionalWhereClause = "[Id] = {request.Id}";
        var deleteStatement = SqlStatementBuilder
            .Delete(entityModel.DbObject.ToString())
            .Where(ReplaceVariables(model.ParamsDto, additionalWhereClause, $"request.{StringHelper.Singularize(model.DbObject.Name)!}"))
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
        var values = GetValues(entityModel.Properties.ExcludeId(), StringHelper.Singularize(model.DbObject.Name)!).ToImmutableArray();
        var idCol = model.ResultDto.Properties.FindId();
        var insertStatement = SqlStatementBuilder
            .Insert()
            .Into(entityModel.DbObject.ToString())
            .Values(values.Select(x => (x.ColumnName, (object)$"{{{x.VariableName}}}"))!)
            .ReturnId()
            .ForceFormatValues(false)
            .Build().Replace(Environment.NewLine, " ").Replace("  ", " ");
        var converterCode = getConverterCodeLine(idCol);
        var result = new StringBuilder()
            .AppendAllLines(values, x => $"var {x.VariableName} = {x.VariableStatement};")
            .AppendLine($"var dbCommand = $@\"{insertStatement}\";")
            .AppendLine($"var dbResult = await this._sql.{nameof(Sql.ExecuteScalarCommandAsync)}(dbCommand, cancellationToken);")
            .AppendLine(converterCode)
            .AppendLine($"var result = new {model.GetSegregateResultType("Command").Name}(returnValue);")
            .AppendLine($"return result;")
            .ToString();
        return result;

        static string getConverterCodeLine(PropertyViewModel? idCol)
        {
            return idCol?.Type switch
            {
                PropertyType.Boolean => $"var returnValue = Convert.ToBoolean(dbResult);",
                PropertyType.String => $"var returnValue = Convert.ToString(dbResult);",
                PropertyType.Integer => $"var returnValue = Convert.ToInt32(dbResult);",
                PropertyType.Long => $"var returnValue = Convert.ToInt64(dbResult);",
                PropertyType.Short => $"var returnValue = Convert.ToInt16(dbResult);",
                PropertyType.Float => $"var returnValue = Convert.ToDecimal(dbResult);",
                PropertyType.Byte => $"var returnValue = Convert.ToByte(dbResult);",
                PropertyType.DateTime => $"var returnValue = Convert.ToDateTime(dbResult);",
                PropertyType.Guid => $"var returnValue = Guid.Parse(dbResult);",                
                PropertyType.Decimal => $"var returnValue = Convert.ToDecimal(dbResult);",
                _ => "var returnValue = dbResult;",
            };
        }
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
        var values = GetValues(entityModel.Properties.ExcludeId(), StringHelper.Singularize(model.DbObject.Name)!).ToImmutableArray();
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

    private static string GenerateApiCallCode(
        in string controllerName,
        in string returnVarStatement = "var apiResult",
        in string? resultTypeName = null,
        in string httpClientInstanceName = "_http",
        in HttpMethod method = HttpMethod.Get,
        in string? nameSpace = null,
        in IEnumerable<string>? queryParams = null,
        in IEnumerable<(string Key, string Value)>? keyValueQueryParams = null,
        in string? paramVarName = null,
        in string? type = null,
        in bool closeStatement = true,
        in bool embrassInToken = true,
        IEnumerable<string>? startLines = null,
        IEnumerable<string>? endLines =  null
        )
    {
        var httpClientMethodName = method switch
        {
            HttpMethod.Get => nameof(HttpClientJsonExtensions.GetFromJsonAsync),
            HttpMethod.Post => nameof(HttpClientJsonExtensions.PostAsJsonAsync),
            HttpMethod.Put => nameof(HttpClientJsonExtensions.PutAsJsonAsync),
            HttpMethod.Delete => nameof(HttpClientJsonExtensions.DeleteFromJsonAsync),
            _ => throw new NotImplementedException()
        };

        // Examples: person = await
        // Http.GetFromJsonAsync<PersonDto>($"HumanResources/person/{personId}");
        // var apiResult = await Http.PostAsJsonAsync("HumanResources/person", newPerson);

        var returnStatement = returnVarStatement.IsNullOrEmpty() ? "" : $"{returnVarStatement} = ";
        var sb = new StringBuilder();
        if (embrassInToken)
        {
            // var token = await _localStorage.GetItemAsync<string>("authToken");
            sb.Append($"""var token = await _localStorage.GetItemAsync<string>("authToken");""");
            //if (!string.IsNullOrEmpty(token))
            //{
            //    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //}
            sb.AppendLine($"""if (!string.IsNullOrEmpty(token))""");
            sb.AppendLine("{");
            sb.AppendLine($"""_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);""");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"""try""");
            sb.AppendLine("{");
        }
        sb.AppendAllLines(startLines);

        // var apiResult = await _http.GetFromJsonAsync
        sb.AppendLine($"""{returnStatement} await {httpClientInstanceName.Trim('.')}.{httpClientMethodName}""");
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>
        sb.Append(resultTypeName is { } value ? $"<{value}>" : "");
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"
        sb.Append("($\"");
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/
        sb.Append(nameSpace?.Trim('/') is { } ns ? $"{ns}/" : null);
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/person/
        sb.Append($"{controllerName.TrimSuffix("Controller").Trim('/').ToLower()}/");
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/person/123456/
        sb.AppendAll(queryParams.Compact().Select(qp => $"{qp.Trim('/')}/"));
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/person/123456/name=ali&age=5
        sb.Append(keyValueQueryParams.Compact(kv => kv is { Key: not null } and { Value: not null }).Select(kv => $"{kv.Key}={kv.Value}").Merge('&'));
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/person/123456/name=ali&age=5"
        sb.Append('"');
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/person/123456/name=ali&age=5", newPerson
        sb.Append(paramVarName.IsNullOrEmpty() ? null : $", {paramVarName}");
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/person/123456/name=ali&age=5", newPerson, type
        sb.Append(type.IsNullOrEmpty() ? null : $", {type}");
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/person/123456/name=ali&age=5", newPerson, type)
        sb.Append(')');
        // var apiResult = await _http.GetFromJsonAsync<PersonDto>($"HumanResources/person/123456/name=ali&age=5", newPerson, type);
        sb.Append(closeStatement ? ';' : null);

        sb.AppendAllLines(endLines);
        if (embrassInToken)
        {
            sb.AppendLine("}");
            sb.AppendLine($"""catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)""");
            sb.AppendLine("{");
            sb.AppendLine($"""_navigationManager.NavigateToLogin("/login");""");
            sb.AppendLine("}");
        }
        var result = sb.ToString();
        return result;
    }

    private static IEnumerable<(string ColumnName, object VariableName, string VariableStatement)> GetValues(IEnumerable<PropertyViewModel> properties, string requestParamName)
    {
        foreach (var p in properties.Compact())
        {
            var dbColumn = p.DbObject.Cast().As<DbColumnViewModel>();
            if (dbColumn is null or { Name: null or { Length: 0 } } or { Type: null or { Length: 0 } })
            {
                continue;
            }

            var type = PropertyTypeHelper.FromDbType(dbColumn.Type);
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

    private enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete
    }
}