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

            var cmd = CodeGenCommandModel.New(model.Name, model.CqrsNameSpace, model.DtoNameSpace, cmdHandler, cmdParams, cmdResult, GetSecurityKeys(model))
                .With(x => x.props().Category = CodeCategory.Command);
            return CodeGenerator.GenerateCode(cmd);
        }
    }

    private static CodeGenDto ConvertViewModelToCodeGen(DtoViewModel resultViewModel)
    {
        var result = CodeGenDto.New(TypePath.New(resultViewModel.NameSpace, resultViewModel.Name).FullPath)
            .With(x => x.IsList = resultViewModel.IsList);
        foreach (var prop in resultViewModel.Properties)
        {
            _ = result.AddProp(CodeGenType.New(prop.TypeFullName), prop.Name!, prop.IsList ?? false, prop.IsNullable ?? false);
        }
        return result;
    }

    private static IEnumerable<string> GetSecurityKeys(CqrsViewModelBase viewModel) =>
            viewModel.SecurityClaims.Select(x => x.Key).Compact() ?? Enumerable.Empty<string>();

    private Codes GenerateQuery(CqrsQueryViewModel model)
    {
        Check.MustBeArgumentNotNull(model?.Name);

        var mainCode = generateMainCode(model);
        var partCode = generatePartCode(model);

        return Codes.New(mainCode.ToCodes().With(x => x.props().Category = mainCode.props().Category)
                       , partCode.ToCodes().With(x => x.props().Category = partCode.props().Category));

        IEnumerable<Code> generateMainCode(CqrsQueryViewModel model)
        {
            var statement = createQueryHandler(model);
            if (statement.IsFailure)
            {
                yield return Code.Empty;
                yield break;
            }
            yield return toCode("Handler", statement, false, CodeCategory.Query);

            Result<string> createQueryHandler(CqrsQueryViewModel model)
            {
                // Create query to be used inside the body code.
                var bodyQuery = SqlStatementBuilder
                    .Select(model.ParamsDto.DbObject.Name!)
                    .Columns(model.ParamsDto.Properties.Select(x => x.DbObject.Name));
                // Create body code.
                var handlerBody = new StringBuilder()
                    .AppendLine($"var dbQuery = @\"{bodyQuery.Build().Replace(Environment.NewLine, " ").Replace("    ", " ")}\";")
                    .AppendLine($"var dbResult = this._sql.Select<{GetResultParam(model).Name}>(dbQuery).ToList();")
                    .AppendLine($"var result = new {GetResultType(model, "Query").Name}(dbResult);")
                    .Append($"return Task.FromResult(result);");

                // Create `HandleAsync` method
                var handleAsyncMethod = new Method(nameof(IQueryHandler<string, string>.HandleAsync))
                {
                    AccessModifier = AccessModifier.Public,
                    Body = handlerBody.Build(),
                    Parameters =
                    {
                        (GetParamsType(model,"Query"), "query")
                    },
                    ReturnType = TypePath.New($"{typeof(Task<>).FullName}<{GetResultType(model, "Query").FullPath}>")
                };
                // Create `QueryHandler` class
                var type = new Class(GetHandlerClassName(model, "Query"))
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
            yield return toCode("Handler", createQueryHandler(model), true, CodeCategory.Query);
            yield return toCode("Params", createQueryParams(model), true, CodeCategory.Dto);
            yield return toCode("Result", createQueryResult(model), true, CodeCategory.Dto);

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
                var type = new Class(GetHandlerClassName(model, "Query"))
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };
                var paramsType = GetParamsType(model, "Query");
                var resultType = GetResultType(model, "Query");
                var baseType = TypePath.New($"{typeof(IQueryHandler<,>).FullName}", new[] { paramsType.FullPath, resultType.FullPath });
                _ = type.BaseTypes.Add(baseType);
                // Add members to `QueryHandler` class
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

            Result<string> createQueryResult(CqrsQueryViewModel model) =>
                createQueryInOut(model.ResultDto, "Result");
            Result<string> createQueryParams(CqrsQueryViewModel model) =>
                createQueryInOut(model.ParamsDto, "Params");
            Result<string> createQueryInOut(DtoViewModel model, string part)
            {
                var className = GetQueryClassName(model, "Query", part);
                var paramsPropType = TypePath.New(model.IsList
                    ? $"IEnumerable<{Purify(model.Name)}{part}>"
                    : $"{Purify(model.Name)}{part}");

                var prop = new CodeGenProperty($"{prp(part)}", paramsPropType);
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
                var nameSpace = INamespace.New(model.NameSpace);
                _ = nameSpace.Types.Add(type);

                // Generate code
                return this._codeGeneratorEngine.Generate(nameSpace);
            }
        }
        Code toCode(string codeName, Result<string> statement, bool isPartial, CodeCategory codeCategory) =>
            Code.New($"{model.Name}{codeName}", Languages.CSharp, statement, isPartial).With(x => x.props().Category = codeCategory);

        static string arg(string name) => TypeMemberNameHelper.ToArgName(name);
        static string fld(string name) => TypeMemberNameHelper.ToFieldName(name);
        static string prp(string name) => TypeMemberNameHelper.ToPropName(name);
    }
}