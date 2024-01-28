using System.Collections.Immutable;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;

using Library.CodeGeneration.Models;
using Library.Results;
using Library.Validations;

using Services.Helpers;

namespace Services;

internal sealed partial class FunctionalityService
{
    public Result<Codes> GenerateCodes(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null)
    {
        Check.MustBeArgumentNotNull(viewModel);

        Result<Codes> result;
        // Determine whether to update existing codes or generate new ones.
        var codeResult = (args?.UpdateModelView ?? false) ? viewModel.Codes : [];
        var scope = ActionScope.Begin(this.Logger, "Generating Functionality code.");

        var results = generateCodes(viewModel, codeResult).ToImmutableArray();
        result = aggregatedResults(results);
        this._reporter.End(result.ToString());
        scope.End(result);
        return result;

        IEnumerable<Result<Codes>> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodes codes)
        {
            var max = 14;
            var index = 0;

            if (viewModel.SourceDto != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.SourceDto);
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.SourceDto)}");
                yield return codes.SourceDtoCodes = codeGenRes;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.GetAllQueryViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.GetAllQueryViewModel);
                codes.GetAllQueryCodes = new(codeGenRes.Select(x => x.Value));
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.GetAllQueryViewModel)}");
                yield return codes.GetAllQueryCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.GetByIdQueryViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.GetByIdQueryViewModel);
                codes.GetByIdQueryCodes = new(codeGenRes.Select(x => x.Value));
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.GetByIdQueryViewModel)}");
                yield return codes.GetByIdQueryCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.InsertCommandViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.InsertCommandViewModel);
                codes.InsertCommandCodes = new(codeGenRes.Select(x => x.Value));
                this._reporter.Report(max, ++index, null);
                yield return codes.InsertCommandCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.UpdateCommandViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.UpdateCommandViewModel);
                codes.UpdateCommandCodes = new(codeGenRes.Select(x => x.Value));
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.UpdateCommandViewModel)}");
                yield return codes.UpdateCommandCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.DeleteCommandViewModel != null)
            {
                var codeGenRes = generateAllCodes(viewModel.DeleteCommandViewModel);
                codes.DeleteCommandCodes = codeGenRes.Select(x => x.Value).ToCodes();
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.DeleteCommandViewModel)}");
                yield return codes.DeleteCommandCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListPageViewModel != null)
            {
                var codeGenRes = this._blazorPageCodeService.GenerateCodes(viewModel.BlazorListPageViewModel);
                codes.BlazorListPageCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorListPageViewModel)}");
                yield return codes.BlazorListPageCodes;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListPageViewModel?.DataContext != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.BlazorListPageViewModel.DataContext);
                codes.BlazorListPageDataContextCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorListPageViewModel.DataContext)}");
                yield return codes.BlazorListPageDataContextCodes;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorDetailsPageViewModel != null)
            {
                var codeGenRes = this._blazorPageCodeService.GenerateCodes(viewModel.BlazorDetailsPageViewModel);
                codes.BlazorDetailsPageCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorDetailsPageViewModel)}");
                yield return codes.BlazorDetailsPageCodes;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorDetailsPageViewModel?.DataContext != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.BlazorDetailsPageViewModel.DataContext);
                codes.BlazorListPageDataContextCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorDetailsPageViewModel.DataContext)}");
                yield return codes.BlazorListPageDataContextCodes;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListComponentViewModel != null)
            {
                var codeGenRes = this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorListComponentViewModel);
                codes.BlazorListComponentCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorListComponentViewModel)}");
                yield return codes.BlazorListComponentCodes;
                if (!codeGenRes.IsSucceed)
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
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorDetailsComponentViewModel)}");
                yield return codes.BlazorDetailsComponentCodes;
                if (!codeGenRes.IsSucceed)
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
                    if (!codeGenRes.IsSucceed)
                    {
                        yield break;
                    }
                }
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.MapperGeneratorViewModel)}");
                codes.BlazorDetailsComponentMapperCodes = Codes.New(mapperCodes);
            }

            // No condition is required.
            {
                var codeGenRes = this._apiCodeGenerator.GenerateCodes(viewModel.ApiCodingViewModel);
                codes.ApiCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.ApiCodingViewModel)}");
                yield return codes.ApiCodes;
                if (!codeGenRes.IsSucceed)
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
                    var paramsDtoCode = this._dtoCodeService.GenerateCodes(model.ParamsDto, new(model.GetSegregateParamsType(kind).Name));
                    // Generate the codes of CQRS result.
                    var resultDtoCode = this._dtoCodeService.GenerateCodes(model.ResultDto, new(model.GetSegregateResultParamsType(kind).Name));
                    // Generate the codes of CQRS handler.
                    var handlerCode = this._cqrsCodeService.GenerateCodes(model);

                    yield return paramsDtoCode;
                    yield return resultDtoCode;
                    yield return handlerCode;
                }
            }
        }

        static Result<Codes> aggregatedResults(IReadOnlyList<Result<Codes>> results)
        {
            Result<Codes> result;
            if (!results.Any())
            {
                result = Result<Codes>.CreateFailure("No codes generated. ViewModel may have no parameter to generate any codes.", Codes.Empty)!;
            }
            else if (results.FirstOrDefault(x => x.IsFailure) is { } failure)
            {
                result = failure;
            }
            else
            {
                result = Result<Codes>.From(results.Combine(), results.Select(x => x.Value).ToCodes());
            }

            return result;
        }
    }
}