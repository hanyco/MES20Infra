using System.Text;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Actors;
using HanyCo.Infra.CodeGeneration.CodeGenerator.AggregatedModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

using Services.Helpers;

using static Services.Helpers.CommonHelpers;

namespace Services;

[Service]
internal sealed class CqrsCodeGeneratorService(ICodeGeneratorEngine codeGenerator) : ICqrsCodeGeneratorService
{
    private readonly ICodeGeneratorEngine _codeGeneratorEngine = codeGenerator;

    public Task<Result<Codes>> GenerateCodesAsync(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null, CancellationToken token = default)
    {
        var result = new Result<Codes>(viewModel.ArgumentNotNull() switch
        {
            CqrsQueryViewModel queryViewModel => this.GenerateQuery(queryViewModel),
            CqrsCommandViewModel commandViewModel => generateCommand(commandViewModel),
            _ => throw new NotSupportedException()
        });
        return Task.FromResult(result);

        static Codes generateCommand(CqrsCommandViewModel model)
        {
            Check.MustBeArgumentNotNull(model?.Name);

            var securityKeys = model.SecurityClaims.ToSecurityKeys();
            var paramsDto = ConvertViewModelToCodeGen(model.ParamsDto)
                .With(x => x.props().Category = CodeCategory.Dto);
            var resultDto = ConvertViewModelToCodeGen(model.ResultDto)
                .With(x => x.props().Category = CodeCategory.Dto);
            var cmdParams = CodeGenCommandParams.New(securityKeys).AddProp(paramsDto, "Params", isList: paramsDto.IsList)
                .With(x => x.props().Category = CodeCategory.Dto);
            var cmdResult = CodeGenCommandResult.New(securityKeys).AddProp(resultDto, "Result", isList: resultDto.IsList)
                .With(x => x.props().Category = CodeCategory.Dto);
            var cmdHandler = CodeGenCommandHandler.New(cmdParams, cmdResult, securityKeys, model.ExecuteBody, (typeof(ICommandProcessor), "CommandProcessor"), (typeof(IQueryProcessor), "QueryProcessor"))
                .With(x => x.props().Category = CodeCategory.Command);

            var cmd = CodeGenCommandModel.New(model.Name, model.CqrsNameSpace, model.DtoNameSpace, cmdHandler, cmdParams, cmdResult, model.GetSecurityKeys())
                .With(x => x.props().Category = CodeCategory.Command);
            return CodeGenerator.GenerateCode(cmd);
        }
        static CodeGenDto ConvertViewModelToCodeGen(DtoViewModel resultViewModel)
        {
            var result = CodeGenDto.New(TypePath.New(resultViewModel.Name, resultViewModel.NameSpace).FullPath)
                .With(x => x.IsList = resultViewModel.IsList);
            foreach (var prop in resultViewModel.Properties)
            {
                _ = result.AddProp(CodeGenType.New(prop.TypeFullName), prop.Name!, prop.IsList ?? false, prop.IsNullable ?? false);
            }
            return result;
        }
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
                .Replace($"^{p!.DbObject?.Name}^", p.Name)
                ;
        }
        return result;
    }

    private Codes GenerateQuery(CqrsQueryViewModel model)
    {
        Check.MustBeArgumentNotNull(model?.Name);

        var mainCode = generateMainCode(model);
        var partCode = generatePartCode(model);

        return Codes.New(mainCode.ToCodes(), partCode.ToCodes());

        IEnumerable<Code> generateMainCode(CqrsQueryViewModel model)
        {
            var statement = createQueryHandler(model);
            if (statement.IsFailure)
            {
                yield return Code.Empty;
                yield break;
            }
            yield return toCode(model.Name, "Handler", statement, false, CodeCategory.Query);

            Result<string> createQueryHandler(CqrsQueryViewModel model)
            {
                var paramName = "query";
                // Create query to be used inside the body code.
                var bodyQuery = SqlStatementBuilder
                    .Select(model.ParamsDto.DbObject.Name!)
                    .SetTopCount(model.ResultDto.IsList ? null : 1)
                    .Where(ReplaceVariables(model.ParamsDto, model.AdditionalSqlStatement.WhereClause, $"{paramName}.Params"))
                    .Columns(model.ResultDto.Properties.Select(x => x.DbObject?.Name).Compact());
                // Create body code.
                (var sqlMethod, var toListMethod) = model.ResultDto.IsList ?
                    (nameof(Sql.Select), ".ToList()") :
                    (nameof(Sql.FirstOrDefault), string.Empty);
                var handlerBody = new StringBuilder()
                    .AppendLine($"var dbQuery = $@\"{bodyQuery.Build().Replace(Environment.NewLine, " ").Replace("  ", " ")}\";")
                    .AppendLine($"var dbResult = this._sql.{sqlMethod}<{model.GetResultParam().Name}>(dbQuery){toListMethod};")
                    .AppendLine($"var result = new {model.GetResultType("Query").Name}(dbResult);")
                    .Append($"return Task.FromResult(result);");

                // Create `HandleAsync` method
                var handleAsyncMethod = new Method(nameof(IQueryHandler<string, string>.HandleAsync))
                {
                    AccessModifier = AccessModifier.Public,
                    Body = handlerBody.Build(),
                    Parameters =
                    {
                        (model.GetParamsType("Query"), paramName)
                    },
                    ReturnType = TypePath.New($"{typeof(Task<>).FullName}<{model.GetResultType("Query").FullPath}>")
                };
                // Create `QueryHandler` class
                var type = new Class(model.GetHandlerClass("Query"))
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };
                _ = type.Members.Add(handleAsyncMethod);

                // Create namespace
                var ns = INamespace.New(model.CqrsNameSpace);
                _ = ns.Types.Add(type);

                // Generate result
                var result = this._codeGeneratorEngine.Generate(ns);
                return result;
            }
        }

        IEnumerable<Code> generatePartCode(CqrsQueryViewModel model)
        {
            yield return toCode(model.Name, "Handler", createQueryHandler(model), true, CodeCategory.Query);
            yield return toCode(model.Name, "Params", createQueryParams(model), true, CodeCategory.Dto);
            yield return toCode(model.Name, "Result", createQueryResult(model), true, CodeCategory.Dto);

            Result<string> createQueryHandler(CqrsQueryViewModel model)
            {
                // Initialize
                var cmdPcr = TypePath.New(typeof(ICommandProcessor));
                var qryPcr = TypePath.New(typeof(IQueryProcessor));
                var dal = TypePath.New(typeof(Sql));

                // Create constructor
                var ctor = new Method(model.Name!)
                {
                    IsConstructor = true,
                    Body = @$"
                        (this.{fld(cmdPcr.Name)}, this.{fld(qryPcr.Name)}) = ({arg(cmdPcr.Name)}, {arg(qryPcr.Name)});
                        this.{fld(dal.Name)} = {arg(dal.Name)};
                        ",
                };
                _ = ctor.AddParameter(cmdPcr.Name, arg(cmdPcr.Name));
                _ = ctor.AddParameter(qryPcr.Name, arg(qryPcr.Name));
                _ = ctor.AddParameter(dal.Name, arg(dal.Name));

                // Create `QueryHandler` class
                var type = new Class(model.GetHandlerClass("Query"))
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };
                var paramsType = model.GetParamsType("Query");
                var resultType = model.GetResultType("Query");
                var baseType = TypePath.New($"{typeof(IQueryHandler<,>).FullName}", new[] { paramsType.FullPath, resultType.FullPath });
                _ = type.BaseTypes.Add(baseType);
                _ = type.AddMember(new Field(fld(cmdPcr.Name), cmdPcr) { IsReadOnly = true });
                _ = type.AddMember(new Field(fld(qryPcr.Name), qryPcr) { IsReadOnly = true });
                _ = type.AddMember(new Field(fld(dal.Name), dal) { IsReadOnly = true });
                _ = type.AddMember(ctor);

                // Create namespace
                var ns = INamespace.New(model.CqrsNameSpace);
                _ = ns.Types.Add(type);

                // Generate code
                var result = this._codeGeneratorEngine.Generate(ns);
                return result;
            }

            Result<string> createQueryResult(CqrsQueryViewModel mode)
            {
                var resultDto = mode.ResultDto;
                var className = resultDto.GetSegregateClassName("Query", "Result");
                var paramsPropType = TypePath.New(resultDto.IsList
                    ? $"List<{Purify(resultDto.Name)}Result>"
                    : $"{Purify(resultDto.Name)}Result");

                var prop = new CodeGenProperty($"{prp("Result")}", paramsPropType);
                var ctor = new Method(className)
                {
                    IsConstructor = true,
                    Body = $"this.{prop.Name} = {arg(prop.Name)};",
                    Parameters =
                    {
                        (paramsPropType, arg(prop.Name))
                    }
                };
                var type = new Class(className).AddMember(ctor, prop);
                var nameSpace = INamespace.New(resultDto.NameSpace);
                _ = nameSpace.Types.Add(type);

                // Generate code
                return this._codeGeneratorEngine.Generate(nameSpace);
            }

            Result<string> createQueryParams(CqrsQueryViewModel model)
            {
                var paramsDto = model.ParamsDto;
                var className = paramsDto.GetSegregateClassName("Query", "Params");
                var paramsPropType = TypePath.New(paramsDto.IsList
                    ? $"IEnumerable<{paramsDto.GetSegregateClassName(null, "Params")}>"
                    : paramsDto.GetSegregateClassName(null, "Params"));

                var prop = new CodeGenProperty(prp("Params"), paramsPropType);
                var ctor = new Method(className)
                {
                    IsConstructor = true,
                    Body = $"this.{prop.Name} = {arg(prop.Name)};",
                    Parameters =
                    {
                        (paramsPropType, arg(prop.Name))
                    }
                };
                var type = new Class(className)
                {
                    BaseTypes =
                    {
                        TypePath.New(typeof(IQuery<>), [model.ResultDto.GetSegregateClassName("Query", "Result")]),
                    }
                }.AddMember(ctor, prop);
                var nameSpace = INamespace.New(paramsDto.NameSpace)
                    .AddType(type);

                // Generate code
                return this._codeGeneratorEngine.Generate(nameSpace);
            }
        }
        static Code toCode(string? modelName, string codeName, Result<string> statement, bool isPartial, CodeCategory codeCategory) =>
            Code.New($"{modelName}{codeName}", Languages.CSharp, statement, isPartial).With(x => x.props().Category = codeCategory);

        static string arg(string name) => TypeMemberNameHelper.ToArgName(name);
        static string fld(string name) => TypeMemberNameHelper.ToFieldName(name);
        static string prp(string name) => TypeMemberNameHelper.ToPropName(name);
    }
}