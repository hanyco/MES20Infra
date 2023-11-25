using System.Collections.Immutable;

using Contracts.Services;
using Contracts.ViewModels;

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
        try
        {
            var results = generateCodes(viewModel, codeResult).ToImmutableArray();
            scope.End("Generated Functionality code.");

            if (!results.Any())
            {
                return Result<Codes>.CreateFailure("No codes generated. ViewModel has no parameter to generate any codes.", Codes.Empty)!;
            };
            if (results.Any(x => x.IsFailure))
            {
                return Result<Codes>.From(results.First(x => x.IsFailure), new(results.Select(x => x.Value)));
            };
            return Result<Codes>.Combine(results, Codes.Combine);
        }
        catch (Exception ex)
        {
            var result = Result<Codes>.CreateFailure(ex, Codes.Empty);
            scope.End(result);
            return result;
        }

        IEnumerable<Result<Codes>> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodes codes)
        {
            if (viewModel.SourceDto != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.SourceDto);
                codes.SourceDtoCodes = codeGenRes;
                yield return codes.SourceDtoCodes;
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
                var codeGenRes = this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel);
                codes.BlazorDetailsComponentCodes = codeGenRes;
                yield return codes.BlazorDetailsComponentCodes;
                if (!codeGenRes)
                {
                    yield break;
                }
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