using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Actors;
using HanyCo.Infra.CodeGeneration.CodeGenerator.AggregatedModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

namespace Services;

[Service]
internal sealed class CqrsCodeGeneratorService : ICqrsCodeGeneratorService
{
    //public IEnumerable<GenerateAllCqrsCodesResultItem> GenerateAllCodes(CqrsGenerateCodesParams parameters, CqrsCodeGenerateCodesConfig? config)
    //{
    //    var (entityName, _, _, _) = parameters.ArgumentNotNull();
    //    config ??= new();
    //    if (config.ShouldGenerateGetAll)
    //    {
    //        yield return new($"GetAll{StringHelper.Pluralize(entityName)}", this.GenerateGetAllCode(parameters));
    //    }
    //    if (config.ShouldGenerateGetById)
    //    {
    //        yield return new($"Get{entityName}ById", this.GenerateGetByIdCode(parameters));
    //    }
    //    if (config.ShouldGenerateCreate)
    //    {
    //        yield return new($"Create{entityName}", this.GenerateCreateCode(parameters));
    //    }
    //    if (config.ShouldGenerateUpdate)
    //    {
    //        yield return new($"Update{entityName}", this.GenerateUpdateCode(parameters));
    //    }
    //    if (config.ShouldGenerateDelete)
    //    {
    //        yield return new($"Delete{entityName}", this.GenerateDeleteCode(parameters));
    //    }
    //}

    public Task<Result<Codes>> GenerateCodeAsync(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null, CancellationToken token = default)
    {
        var result = new Result<Codes>(viewModel.ArgumentNotNull() switch
        {
            CqrsQueryViewModel queryViewModel => GenerateQuery(queryViewModel),
            CqrsCommandViewModel commandViewModel => GenerateCommand(commandViewModel),
            _ => throw new NotSupportedException()
        });
        return Task.FromResult(result);
    }

    //UNDONE CQRS Code Generator Service SaveToDatabaseAsync.
    public Task SaveToDatabaseAsync(CqrsGenerateCodesParams parameters, CqrsCodeGenerateCodesConfig config, CancellationToken token = default) =>
        throw new NotImplementedException();

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

    private static Codes GenerateCommand(CqrsCommandViewModel commandViewModel)
    {
        Check.MustBeArgumentNotNull(commandViewModel?.Name);

        var paramsDto = ConvertViewModelToCodeGen(commandViewModel.ParamDto);
        var resultDto = ConvertViewModelToCodeGen(commandViewModel.ResultDto);
        var cmdParam = CodeGenCommandParameter.New().AddProp(paramsDto, "dto").With(x => x.props().Category = CodeCategory.Dto);
        var cmdResult = CodeGenCommandResult.New().AddProp(resultDto, "Result").With(x => x.props().Category = CodeCategory.Dto);
        var cmdHandler = CodeGenCommandHandler.New(cmdParam, cmdResult).With(x => x.props().Category = CodeCategory.Command);

        var cmd = CodeGenCommandModel.New(commandViewModel.Name,
                                          commandViewModel.CqrsNameSpace,
                                          commandViewModel.DtoNameSpace,
                                          cmdHandler,
                                          cmdParam,
                                          cmdResult)
                                     .With(x => x.props().Category = CodeCategory.Command);
        return CodeGenerator.GenerateCode(cmd);
    }

    private static Codes GenerateQuery(CqrsQueryViewModel queryViewModel)
    {
        Check.MustBeArgumentNotNull(queryViewModel?.Name);

        var paramsDto = ConvertViewModelToCodeGen(queryViewModel.ParamDto);
        var resultDto = ConvertViewModelToCodeGen(queryViewModel.ResultDto);
        var qryParams = CodeGenQueryParam.New().AddProp(paramsDto, "dto", isList: resultDto.IsList).With(x => x.props().Category = CodeCategory.Dto);
        var qryResult = CodeGenQueryResult.New().AddProp(resultDto, "Result", isList: resultDto.IsList).With(x => x.props().Category = CodeCategory.Dto);
        var qryHandler = CodeGenQueryHandler.New(qryParams,
                                                   qryResult,
                                                   (typeof(ICommandProcessor), "CommandProcessor"),
                                                   (typeof(IQueryProcessor), "QueryProcessor"))
                                              .With(x => x.props().Category = CodeCategory.Query);

        var qry = CodeGenQueryModel.New(queryViewModel.Name, queryViewModel.CqrsNameSpace, queryViewModel.DtoNameSpace, qryHandler, qryParams, qryResult, paramsDto, resultDto)
                                   .With(x => x.props().Category = CodeCategory.Query);
        return CodeGenerator.GenerateCode(qry);
    }
}