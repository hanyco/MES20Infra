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

    [Obsolete]
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

    [Obsolete]
    private static CodeGenDto ConvertViewModelToCodeGen(DtoViewModel resultViewModel)
    {
        var result = CodeGenDto.New(TypeMemberNameHelper.GetFullName(resultViewModel.NameSpace, resultViewModel.Name))
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

        var mainCode = Code.New(model.Name, Languages.CSharp, generateMainCode(model).ThrowOnFail(), false);
        var partCode = Code.New(model.Name, Languages.CSharp, generatePartCode(model).ThrowOnFail(), true);

        return Codes.New(mainCode
                .With(x => x.props().Category = CodeCategory.Query)
            , partCode
                .With(x => x.props().Category = CodeCategory.Query))
            .With(x => x.props().Category = CodeCategory.Query);

        Result<string> generateMainCode(CqrsQueryViewModel model)
        {
            // Create query to be used inside the body code.
            var bodyQuery = SqlStatementBuilder
                .Select(model.ParamsDto.DbObject.Name)
                .Columns(model.ParamsDto.Properties.Select(x => x.DbObject.Name));
            // Create body code.
            var handlerBody = new StringBuilder()
                .AppendLine($"var dbQuery = @\"{bodyQuery.Build()}\";")
                .AppendLine($"var dbResult = this._sql.Select<{GetResultParam(model).Name}>(dbQuery).ToList();")
                .AppendLine($"var result = new {GetResultType(model).Name}(dbResult);")
                .Append($"return Task.FromResult(result);");

            // Create `ExecuteAsync` method
            var handleAsyncMethod = new Method(nameof(IQueryHandler<string, string>.HandleAsync))
            {
                AccessModifier = AccessModifier.Public,
                Body = handlerBody.Build(),
                Parameters =
                    {
                        (GetParamsType(model), "query")
                    },
                ReturnType = TypePath.New($"{typeof(Task<>).FullName}<{GetResultType(model).FullPath}>")
            };
            // Create `QueryHandler` class
            var type = new Class(GetHandlerClassName(model))
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

        Result<string> generatePartCode(CqrsQueryViewModel model)
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
            var type = new Class(GetHandlerClassName(model))
            {
                AccessModifier = AccessModifier.Public,
                InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
            };
            var baseType = TypePath.New($"{typeof(IQueryHandler<,>).FullName}", new[] { GetParamsType(model).FullPath, GetResultParam(model).FullPath });
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

        static string arg(string name) => TypeMemberNameHelper.ToArgName(name);
        static string fld(string name) => TypeMemberNameHelper.ToFieldName(name);
    }
}