using System.Collections;
using System.Collections.Immutable;
using System.Text;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.Services;
using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2.Back;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

using Services.Helpers;

namespace Services;

internal partial class FunctionalityService
{
    public Result<Codes?> GenerateCodes(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null)
    {
        Check.MustBeArgumentNotNull(viewModel);

        Result<Codes?> result;
        // To determine whether to update existing codes or generate new ones.
        var codeResult = (args?.UpdateModelView ?? false) ? viewModel.Codes : [];
        var scope = ActionScope.Begin(this.Logger, "Generating Functionality code.");

        var results = generateCodes(viewModel, codeResult).ToImmutableArray();
        result = aggregatedResults(results);
        if (result.IsFailure || (result.Value?.Count < 1))
        {
            return result;
        }
        this._reporter.End(result.ToString());
        var cats = result.Value.Select(x => x.GetCategory());
        scope.End(result);
        return result;

        IEnumerable<Result<Codes>> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodes codes)
        {
            var (index, max) = (0, 14);

            if (viewModel.SourceDto != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.SourceDto);
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.SourceDto)}");
                yield return codes.SourceDtoCodes = codeGenRes!;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.GetAllQueryViewModel != null)
            {
                var codeGenRes = generateCqrsCodes(viewModel.GetAllQueryViewModel);
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
                var codeGenRes = generateCqrsCodes(viewModel.GetByIdQueryViewModel);
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
                var codeGenRes = generateCqrsCodes(viewModel.InsertCommandViewModel);
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
                var codeGenRes = generateCqrsCodes(viewModel.UpdateCommandViewModel);
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
                var codeGenRes = generateCqrsCodes(viewModel.DeleteCommandViewModel);
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
                yield return codes.BlazorListPageCodes!;
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
                yield return codes.BlazorListPageDataContextCodes!;
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
                yield return codes.BlazorDetailsPageCodes!;
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
                yield return codes.BlazorListPageDataContextCodes!;
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
                yield return codes.BlazorListComponentCodes!;
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
                yield return codes.BlazorDetailsComponentCodes!;
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
                yield return codes.ApiCodes!;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            ImmutableArray<Result<Codes>> generateCqrsCodes(CqrsViewModelBase cqrsViewModel)
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
                    var paramsDtoCode = generateCodes(model.ParamsDto, new(model.GetSegregateParamsType(kind).Name));
                    // Generate the codes of CQRS result.
                    var resultDtoCode = generateCodes(model.ResultDto, new(model.GetSegregateResultParamsType(kind).Name));
                    // Generate the codes of CQRS handler.
                    var handlerCode = this._cqrsCodeService.GenerateCodes(model, new(false, true, false));

                    yield return paramsDtoCode.ToCodes();
                    yield return resultDtoCode.ToCodes();
                    yield return handlerCode!;

                    Code generateCodes(DtoViewModel dto, string typeName)
                    {
                        var type = new Class(typeName)
                        {
                            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                        };
                        if (dto.BaseType is not null)
                        {
                            _ = type.AddBaseType(dto.BaseType);
                        }
                        var ctor = new Method(typeName);
                        var ctorBody = new StringBuilder(string.Empty);
                        foreach (var property in dto.Properties)
                        {
                            var prop = property.IsList ?? false
                                ? new CodeGenProperty(property.Name!, TypePath.New<IList>([property.TypeFullName]))
                                : new CodeGenProperty(property.Name!, property.TypeFullName);
                            _ = type.AddMember(prop);
                            var argName = TypeMemberNameHelper.ToArgName(prop.Name);
                            _ = ctor.AddArgument(prop.Type, argName);
                            _ = ctorBody.AppendLine($"this.{prop.Name} = {argName};");
                        }
                        if (ctorBody.Length != 0)
                        {
                            ctor.Body = ctorBody.ToString();
                            _ = type.AddMember(ctor);
                            var defCtor = new Method(typeName).With(x => x.Body = "// Default constructor");
                            _ = type.AddMember(defCtor);
                        }
                        var ns = INamespace.New(dto.NameSpace).AddType(type);
                        var codeStatement = this._generatorEngine.Generate(ns);
                        var code = Code.New(typeName, Languages.CSharp, codeStatement, true).SetCategory(CodeCategory.Dto);
                        return code;
                    }
                }
            }
        }

        static Result<Codes?> aggregatedResults(IReadOnlyList<Result<Codes>> results)
        {
            Result<Codes?> result;
            if (!results.Any())
            {
                result = Result.Fail<Codes>("No codes generated. Maybe ViewModel has no parameter to generate any codes.");
            }
            else if (results.FirstOrDefault(x => x.IsFailure) is { } failure)
            {
                result = failure!;
            }
            else
            {
                result = Result.From(results.Combine(), results.Select(x => x.Value).ToCodes())!;
            }

            return result;
        }
    }
}