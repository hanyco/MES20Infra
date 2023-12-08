using System.Collections.Immutable;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Results;
using Library.Validations;

using Newtonsoft.Json.Linq;

using Services.Helpers;

namespace Services;

internal sealed partial class FunctionalityService
{
    public Result<Codes> GenerateCodes(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null)
    {
        Check.MustBeArgumentNotNull(viewModel);

        // Determine whether to update existing codes or generate new ones.
        var codeResult = (args?.UpdateModelView ?? false) ? viewModel.Codes : [];

        var scope = ActionScope.Begin(this.Logger, "Generating Functionality code.");
        Result<Codes> result = default!;
        try
        {
            var results = generateCodes(viewModel, codeResult).ToImmutableArray();
            if (!results.Any())
            {
                result = Result<Codes>.CreateFailure("No codes generated. ViewModel may have no parameter to generate any codes.", Codes.Empty)!;
            }
            else if (results.Any(x => x.IsFailure))
            {
                result = Result<Codes>.From(results.First(x => x.IsFailure), new(results.Select(x => x.Value)));
            }
            else
            {
                result = Result<Codes>.From(results.Combine(), results.Select(x => x.Value).ToCodes());
            }
        }
        catch (Exception ex)
        {
            result = Result<Codes>.CreateFailure(ex, Codes.Empty);
        }
        finally
        {
            scope.End(result);
        }
        return result;

        IEnumerable<Result<Codes>> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodes codes)
        {
            if (viewModel.SourceDto != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.SourceDto);
                yield return codes.SourceDtoCodes = codeGenRes;
                if (!codeGenRes)
                {
                    yield break;
                }
            }

            if (viewModel.GetAllQueryViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.GetAllQueryViewModel);
                codes.GetAllQueryCodes = new(codeGenRes.Select(x => x.Value));
                yield return codes.GetAllQueryCodes;
                if (codeGenRes.Any(x => !x))
                {
                    yield break;
                }
            }

            if (viewModel.GetByIdQueryViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.GetByIdQueryViewModel);
                codes.GetByIdQueryCodes = new(codeGenRes.Select(x => x.Value));
                yield return codes.GetByIdQueryCodes;
                if (codeGenRes.Any(x => !x))
                {
                    yield break;
                }
            }

            if (viewModel.InsertCommandViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.InsertCommandViewModel);
                codes.InsertCommandCodes = new(codeGenRes.Select(x => x.Value));
                yield return codes.InsertCommandCodes;
                if (codeGenRes.Any(x => !x))
                {
                    yield break;
                }
            }

            if (viewModel.UpdateCommandViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.UpdateCommandViewModel);
                codes.UpdateCommandCodes = new(codeGenRes.Select(x => x.Value));
                yield return codes.UpdateCommandCodes;
                if (codeGenRes.Any(x => !x))
                {
                    yield break;
                }
            }

            if (viewModel.DeleteCommandViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.DeleteCommandViewModel);
                codes.DeleteCommandCodes = new(codeGenRes.Select(x => x.Value));
                yield return codes.DeleteCommandCodes;
                if (codeGenRes.Any(x => !x))
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListPageViewModel != null)
            {
                var codeGenRes = this._blazorPageCodeService.GenerateCodes(viewModel.BlazorListPageViewModel);
                codes.BlazorListPageCodes = codeGenRes;
                yield return codes.BlazorListPageCodes;
                if (!codeGenRes)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListPageViewModel?.DataContext != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.BlazorListPageViewModel.DataContext);
                codes.BlazorListPageDataContextCodes = codeGenRes;
                yield return codes.BlazorListPageDataContextCodes;
                if (!codeGenRes)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorDetailsPageViewModel != null)
            {
                var codeGenRes = this._blazorPageCodeService.GenerateCodes(viewModel.BlazorDetailsPageViewModel);
                codes.BlazorDetailsPageCodes = codeGenRes;
                yield return codes.BlazorDetailsPageCodes;
                if (!codeGenRes)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorDetailsPageViewModel?.DataContext != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.BlazorDetailsPageViewModel.DataContext);
                codes.BlazorListPageDataContextCodes = codeGenRes;
                yield return codes.BlazorListPageDataContextCodes;
                if (!codeGenRes)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListComponentViewModel != null)
            {
                var codeGenRes = this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorListComponentViewModel);
                codes.BlazorListComponentCodes = codeGenRes;
                yield return codes.BlazorListComponentCodes;
                if (!codeGenRes)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorDetailsComponentViewModel != null)
            {
                var editForm = viewModel.BlazorDetailsComponentViewModel.EditFormInfo;
                var args = new GenerateCodesParameters(
                    IsEditForm: editForm.IsEditForm,
                    EditFormAttributes: editForm.Events.Select(x => (x.Name, x.Handler.Name)).AddImmuted(("Model", editForm.Model))
                    );
                var codeGenRes = this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel, args);
                codes.BlazorDetailsComponentCodes = codeGenRes;
                yield return codes.BlazorDetailsComponentCodes;
                if (!codeGenRes)
                {
                    yield break;
                }
            }

            if (viewModel.MapperGeneratorViewModel.Arguments.Count != 0)
            {
                var mapperCodes = new List<Codes>();
                foreach (var argument in viewModel.MapperGeneratorViewModel.Arguments)
                {
                    var codeGenRes = this._mapperSourceGenerator.GenerateCodes(argument);
                    mapperCodes.Add(codeGenRes);
                    yield return codeGenRes;
                    if (!codeGenRes)
                    {
                        yield break;
                    }
                }
                codes.BlazorDetailsComponentMapperCodes = Codes.New(mapperCodes);
            }

            ImmutableArray<Result<Codes>> generateAllCodes(CqrsViewModelBase cqrsViewModel)
            {
                return gather(cqrsViewModel).ToImmutableArray();

                IEnumerable<Result<Codes>> gather(CqrsViewModelBase model)
                {
                    var kind = model switch
                    {
                        CqrsQueryViewModel => "Query",
                        CqrsCommandViewModel => "Command",
                        _ => throw new NotImplementedException()
                    };
                    // Generate the codes of CQRS parameters.
                    var paramsDtoCodeResult = this._dtoCodeService.GenerateCodes(model.ParamsDto, new(model.GetSegregateParamsType(kind).Name));
                    // Generate the codes of CQRS result.
                    var resultDtoCodeResult = this._dtoCodeService.GenerateCodes(model.ResultDto, new(model.GetSegregateResultParamsType(kind).Name));
                    // Generate the codes of CQRS handler.
                    var handlerCodeResult = this._cqrsCodeService.GenerateCodes(model);

                    yield return paramsDtoCodeResult;
                    yield return resultDtoCodeResult;
                    yield return handlerCodeResult;
                }
            }
        }
    }
}