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

namespace Services;

[Service]
internal sealed class CqrsCodeGeneratorService(ICodeGeneratorEngine codeGenerator) : ICqrsCodeGeneratorService
{
    private readonly ICodeGeneratorEngine _codeGenerator = codeGenerator;

    public static TypePath GetParamsName(CqrsQueryViewModel model) => TypePath.New($"{model.DbObject.Name}QueryParameter", model.ParamsDto.NameSpace);

    public static TypePath GetParamsName(CqrsCommandViewModel model) => TypePath.New($"{model.DbObject.Name}CommandParameter", model.ParamsDto.NameSpace);

    public static TypePath GetResultName(CqrsQueryViewModel model) => TypePath.New($"{model.DbObject.Name}QueryResult", model.ResultDto.NameSpace);

    public static TypePath GetResultName(CqrsCommandViewModel model) => TypePath.New($"{model.DbObject.Name}CommandResult", model.ResultDto.NameSpace);

    [Obsolete]
    public Task<Result<Codes>> GenerateCodesAsync(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null, CancellationToken token = default)
    {
        var result = new Result<Codes>(viewModel.ArgumentNotNull() switch
        {
            CqrsQueryViewModel queryViewModel => generateQuery(queryViewModel),
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

        Codes generateQuery(CqrsQueryViewModel model)
        {
            Check.MustBeArgumentNotNull(model?.Name);

            var mainCode = Code.New(model.Name, Languages.CSharp, generateMainCode(model).ThrowOnFail(), false);
            var partCode = Code.New(model.Name, Languages.CSharp, generatePartCode(model).ThrowOnFail(), true);

            return Codes.New(mainCode, partCode);

            Result<string> generateMainCode(CqrsQueryViewModel model)
            {
                var type = new Class(getHandlerClassName(model))
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };
                var handleAsyncMethod = new Method(nameof(IQueryHandler<string, string>.HandleAsync))
                {
                    AccessModifier = AccessModifier.Public,
                    // UNDO: Replace with actual GetAll code.
                    Body = @$"
                        var dbTable = ""{model.ParamsDto.DbObject}"";
                        var query = = SqlStatementBuilder
                            .Select()
                            //.Columns()
                            //.OrderBy(""Id"")
                            .Descending()
                            .Build();
                        throw new System.NotImplementedException();
                        ",
                    Parameters =
                    {
                        (GetParamsName(model), "query")
                    },
                    ReturnType = TypePath.New($"{typeof(Task<>).FullName}<{GetResultName(model).Name}>")
                };
                _ = type.Members.Add(handleAsyncMethod);
                var ns = INamespace.New(model.CqrsNameSpace);
                _ = ns.Types.Add(type);
                return this._codeGenerator.Generate(ns);
            }
            Result<string> generatePartCode(CqrsQueryViewModel model)
            {
                var ns = INamespace.New(model.CqrsNameSpace);
                var type = new Class(getHandlerClassName(model))
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };
                var baseType = TypePath.New($"{typeof(IQueryHandler<,>).FullName}", new[] { GetParamsName(model).FullPath, GetResultName(model).FullPath });
                _ = type.BaseTypes.Add(baseType);

                var cmdPcr = TypePath.New(typeof(ICommandProcessor));
                var qryPcr = TypePath.New(typeof(IQueryProcessor));
                var dal = TypePath.New(typeof(Sql));
                var ctor = new Method(model.Name!)
                {
                    IsConstructor = true,
                    Body = @$"
                        (this.{fld(cmdPcr.Name)}, this.{fld(qryPcr.Name)}) = ({arg(cmdPcr.Name)}, {arg(qryPcr.Name)});
                        this.{fld(dal.Name)} = {arg(dal.Name)};
                        ",
                };

                _ = type.AddMember(new Field(fld(cmdPcr.Name), cmdPcr) { IsReadOnly = true });
                _ = ctor.AddParameter(cmdPcr.Name, arg(cmdPcr.Name));

                _ = type.AddMember(new Field(fld(qryPcr.Name), qryPcr) { IsReadOnly = true });
                _ = ctor.AddParameter(qryPcr.Name, arg(qryPcr.Name));

                _ = type.AddMember(new Field(fld(dal.Name), dal) { IsReadOnly = true });
                _ = ctor.AddParameter(dal.Name, arg(dal.Name));

                _ = type.AddMember(ctor);

                _ = ns.Types.Add(type);
                var partCode = this._codeGenerator.Generate(ns);
                return partCode;
            }

            static string arg(string name) => TypeMemberNameHelper.ToArgName(name);
            static string fld(string name) => TypeMemberNameHelper.ToFieldName(name);
            static TypePath getHandlerClassName(CqrsQueryViewModel model) => TypePath.New($"{model.Name}Handler", model.CqrsNameSpace);
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
}