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
    public Task<Result<Codes>> GenerateCodeAsync(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null, CancellationToken token = default)
    {
        var result = new Result<Codes>(viewModel.ArgumentNotNull() switch
        {
            CqrsQueryViewModel queryViewModel => generateQuery(queryViewModel),
            CqrsCommandViewModel commandViewModel => generateCommand(commandViewModel),
            _ => throw new NotSupportedException()
        });
        return Task.FromResult(result);

        static Codes generateCommand(CqrsCommandViewModel commandViewModel)
        {
            Check.MustBeArgumentNotNull(commandViewModel?.Name);

            var paramsDto = ConvertViewModelToCodeGen(commandViewModel.ParamsDto)
                .With(x => x.props().Category = CodeCategory.Dto);
            var resultDto = ConvertViewModelToCodeGen(commandViewModel.ResultDto)
                .With(x => x.props().Category = CodeCategory.Dto);
            var cmdParams = CodeGenCommandParams.New().AddProp(paramsDto, "Params", isList: paramsDto.IsList)
                .With(x => x.props().Category = CodeCategory.Dto);
            var cmdResult = CodeGenCommandResult.New().AddProp(resultDto, "Result", isList: resultDto.IsList)
                .With(x => x.props().Category = CodeCategory.Dto);
            var cmdHandler = CodeGenCommandHandler.New(cmdParams, cmdResult, (typeof(ICommandProcessor), "CommandProcessor"), (typeof(IQueryProcessor), "QueryProcessor"))
                .With(x => x.props().Category = CodeCategory.Command);

            var cmd = CodeGenCommandModel.New(commandViewModel.Name, commandViewModel.CqrsNameSpace, commandViewModel.DtoNameSpace, cmdHandler, cmdParams, cmdResult)
                .With(x => x.props().Category = CodeCategory.Command);
            return CodeGenerator.GenerateCode(cmd);
        }

        static Codes generateQuery(CqrsQueryViewModel queryViewModel)
        {
            Check.MustBeArgumentNotNull(queryViewModel?.Name);

            var paramsDto = ConvertViewModelToCodeGen(queryViewModel.ParamsDto)
                .With(x => x.props().Category = CodeCategory.Dto);
            var resultDto = ConvertViewModelToCodeGen(queryViewModel.ResultDto)
                .With(x => x.props().Category = CodeCategory.Dto);
            var qryParams = CodeGenQueryParams.New().AddProp(paramsDto, "Params", isList: paramsDto.IsList)
                .With(x => x.props().Category = CodeCategory.Dto);
            var qryResult = CodeGenQueryResult.New().AddProp(resultDto, "Result", isList: resultDto.IsList)
                .With(x => x.props().Category = CodeCategory.Dto);
            var qryHandler = CodeGenQueryHandler.New(qryParams, qryResult, (typeof(ICommandProcessor), "CommandProcessor"), (typeof(IQueryProcessor), "QueryProcessor"))
                .With(x => x.props().Category = CodeCategory.Query);

            var qry = CodeGenQueryModel.New(queryViewModel.Name, queryViewModel.CqrsNameSpace, queryViewModel.DtoNameSpace, qryHandler, qryParams, qryResult)
                .With(x => x.props().Category = CodeCategory.Query);
            return CodeGenerator.GenerateCode(qry);
        }
    }

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
}