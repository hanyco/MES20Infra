using System.Collections.Immutable;

using Contracts.Services;
using Contracts.ViewModels;

using Library.CodeGeneration.Models;
using Library.Results;
using Library.Validations;

using Newtonsoft.Json.Linq;

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
            var results = generateCodes(viewModel, codeResult);
            scope.End("Generated Functionality code.");

            return results.Any()
                ? Result<Codes>.Combine(results, Codes.Combine)
                : Result<Codes>.CreateFailure("No codes generated. ViewModel has no parameter to generate any codes.", Codes.Empty)!;
        }
        catch (Exception ex)
        {
            var result = Result<Codes>.CreateFailure(ex, Codes.Empty);
            scope.End(result);
            return result;
        }

        IEnumerable<Result<Codes>> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodes codes)
        {
            var result = new List<Result<Codes>>();

            if (viewModel.SourceDto != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.SourceDto}");
                var codeGenRes = addToResult(this._dtoCodeService.GenerateCodes(viewModel.SourceDto));
                if (!codeGenRes)
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.SourceDto}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.SourceDto}");
                codes.SourceDtoCodes = codeGenRes;
            }

            if (viewModel.GetAllQueryViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.GetAllQueryViewModel}");
                var codeGenRes = generateAllCodes(viewModel.GetAllQueryViewModel);
                if (codeGenRes.Any(x => !x))
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.SourceDto}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.GetAllQueryViewModel}");
                codes.GetAllQueryCodes = new(codeGenRes.Select(x => x.Value));
            }

            if (viewModel.GetByIdQueryViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.GetByIdQueryViewModel}");
                var codeGenRes = generateAllCodes(viewModel.GetByIdQueryViewModel);
                if (codeGenRes.Any(x => !x))
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.GetByIdQueryViewModel}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.GetByIdQueryViewModel}");
                codes.GetByIdQueryCodes = new(codeGenRes.Select(x => x.Value));
            }

            if (viewModel.InsertCommandViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.InsertCommandViewModel}");
                var codeGenRes = generateAllCodes(viewModel.InsertCommandViewModel);
                if (codeGenRes.Any(x => !x))
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.InsertCommandViewModel}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.InsertCommandViewModel}");
                codes.InsertCommandCodes = new(codeGenRes.Select(x => x.Value));
            }

            if (viewModel.UpdateCommandViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.UpdateCommandViewModel}");
                var codeGenRes = generateAllCodes(viewModel.UpdateCommandViewModel);
                if (codeGenRes.Any(x => !x))
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.UpdateCommandViewModel}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.UpdateCommandViewModel}");
                codes.UpdateCommandCodes = new(codeGenRes.Select(x => x.Value));
            }

            if (viewModel.DeleteCommandViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.DeleteCommandViewModel}");
                var codeGenRes = generateAllCodes(viewModel.DeleteCommandViewModel);
                if (codeGenRes.Any(x => !x))
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.DeleteCommandViewModel}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.DeleteCommandViewModel}");
                codes.DeleteCommandCodes = new(codeGenRes.Select(x => x.Value));
            }

            if (viewModel.BlazorListPageViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorListPageViewModel}");
                var codeGenRes = addToResult(this._blazorPageCodeService.GenerateCodes(viewModel.BlazorListPageViewModel));
                if (!codeGenRes)
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.BlazorListPageViewModel}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorListPageViewModel}");
                codes.BlazorListPageCodes = codeGenRes;
            }

            if (viewModel.BlazorListPageViewModel?.DataContext != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorListPageViewModel.DataContext}");
                var codeGenRes = addToResult(this._dtoCodeService.GenerateCodes(viewModel.BlazorListPageViewModel.DataContext));
                if (!codeGenRes)
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.BlazorListPageViewModel.DataContext}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorListPageViewModel.DataContext}");
                codes.BlazorListPageDataContextCodes = codeGenRes;
            }

            if (viewModel.BlazorDetailsPageViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorDetailsPageViewModel}");
                var codeGenRes = addToResult(this._blazorPageCodeService.GenerateCodes(viewModel.BlazorDetailsPageViewModel));
                if (!codeGenRes)
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.BlazorDetailsPageViewModel}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorDetailsPageViewModel}");
                codes.BlazorDetailsPageCodes = codeGenRes;
            }

            if (viewModel.BlazorDetailsPageViewModel?.DataContext != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorDetailsPageViewModel.DataContext}");
                var codeGenRes = addToResult(this._dtoCodeService.GenerateCodes(viewModel.BlazorDetailsPageViewModel.DataContext));
                if (!codeGenRes)
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.BlazorDetailsPageViewModel.DataContext}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorDetailsPageViewModel.DataContext}");
                codes.BlazorListPageDataContextCodes = codeGenRes;
            }

            if (viewModel.BlazorListComponentViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorListComponentViewModel}");
                var codeGenRes = addToResult(this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorListComponentViewModel));
                if (!codeGenRes)
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.BlazorListComponentViewModel}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorListComponentViewModel}");
                codes.BlazorListComponentCodes = codeGenRes;
            }

            if (viewModel.BlazorDetailsComponentViewModel != null)
            {
                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorDetailsComponentViewModel}");
                var codeGenRes = addToResult(this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel));
                if (!codeGenRes)
                {
                    this.Logger.Debug($"Not generated Functionality code for {viewModel.BlazorDetailsComponentViewModel}. Error: {codeGenRes}");
                    return result;
                }

                this.Logger.Debug($"Generating Functionality code for {viewModel.BlazorDetailsComponentViewModel}");
                codes.BlazorDetailsComponentCodes = codeGenRes;
            }

            return result;

            // Internal method to add a code result to the result list.
            Result<Codes> addToResult(Result<Codes> codeResult) =>
                codeResult.Fluent(result.Add);

            ImmutableArray<Result<Codes>> generateAllCodes(CqrsViewModelBase cqrsViewModel)
            {
                return gather(cqrsViewModel, result).ToImmutableArray();

                IEnumerable<Result<Codes>> gather(CqrsViewModelBase cqrsViewModel, List<Result<Codes>>? result)
                {
                    // Generate the codes of CQRS parameters.
                    var paramsDtoCodeResult = this._dtoCodeService.GenerateCodes(cqrsViewModel.ParamsDto);
                    // Generate the codes of CQRS result.
                    var resultDtoCodeResult = this._dtoCodeService.GenerateCodes(cqrsViewModel.ResultDto);
                    // Generate the codes of CQRS handler.
                    var handlerCodeResult = this._cqrsCodeService.GenerateCodes(cqrsViewModel);

                    //yield return addToResult(paramsDtoCodeResult);
                    //yield return addToResult(resultDtoCodeResult);
                    yield return addToResult(handlerCodeResult);
                }
            }
        }
    }
}