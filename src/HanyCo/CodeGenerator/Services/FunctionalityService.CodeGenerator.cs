using HanyCo.Infra.CodeGen.Contracts.CodeGen.Services;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2.Back;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

using Services.Helpers;

using System.Collections.Immutable;
using System.Text;

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

            if (viewModel.GetAllQuery != null)
            {
                var codeGenRes = generateCqrsCodes(viewModel.GetAllQuery);
                codes.GetAllQueryCodes = new(codeGenRes.Select(x => x.Value));
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.GetAllQuery)}");
                yield return codes.GetAllQueryCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.GetByIdQuery != null)
            {
                var codeGenRes = generateCqrsCodes(viewModel.GetByIdQuery);
                codes.GetByIdQueryCodes = new(codeGenRes.Select(x => x.Value));
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.GetByIdQuery)}");
                yield return codes.GetByIdQueryCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.InsertCommand != null)
            {
                var codeGenRes = generateCqrsCodes(viewModel.InsertCommand);
                codes.InsertCommandCodes = new(codeGenRes.Select(x => x.Value));
                this._reporter.Report(max, ++index, null);
                yield return codes.InsertCommandCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.UpdateCommand != null)
            {
                var codeGenRes = generateCqrsCodes(viewModel.UpdateCommand);
                codes.UpdateCommandCodes = new(codeGenRes.Select(x => x.Value));
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.UpdateCommand)}");
                yield return codes.UpdateCommandCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.DeleteCommand != null)
            {
                var codeGenRes = generateCqrsCodes(viewModel.DeleteCommand);
                codes.DeleteCommandCodes = codeGenRes.Select(x => x.Value).ToCodes();
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.DeleteCommand)}");
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
                    var paramsDtoCode = createParams(model);
                    var resultDtoCode = createResult(model);
                    var handlerCode = createHandler(model);

                    yield return paramsDtoCode.ToCodes();
                    yield return resultDtoCode.ToCodes();
                    yield return handlerCode.ToCodes();

                    Code createHandler(CqrsViewModelBase model)
                    {
                        var category = model switch
                        {
                            CqrsQueryViewModel => CodeCategory.Query,
                            CqrsCommandViewModel => CodeCategory.Command,
                            _ => throw new NotSupportedException()
                        };
                        var handlerClass = new Class(model.GetSegregateHandlerType(null!))
                        {
                            AccessModifier = AccessModifier.Internal,
                            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                        };

                        var cmdPcr = TypePath.New<ICommandProcessor>();
                        var qryPcr = TypePath.New<IQueryProcessor>();
                        var dal = TypePath.New<Sql>();

                        var ctor = new Method(model.GetSegregateHandlerType(null!))
                        {
                            IsConstructor = true,
                            Arguments =
                            {
                                new(cmdPcr, arg(cmdPcr.Name)),
                                new(qryPcr, arg(qryPcr.Name)),
                                new(dal, arg(dal.Name))
                            },
                            Body = $"""
                                    (this.{fld(cmdPcr.Name)}, this.{fld(qryPcr.Name)}) = ({arg(cmdPcr.Name)}, {arg(qryPcr.Name)});
                                    this.{fld(dal.Name)} = {arg(dal.Name)};
                                    """
                        };
                        
                        var paramsType = model.GetSegregateParamsType(null!);
                        var resultType = model.GetSegregateResultParamsType(null!);
                        var baseType = category switch
                        {
                            CodeCategory.Query => TypePath.New(typeof(IQueryHandler<,>).FullName!, [paramsType.FullName, resultType.FullName]),
                            CodeCategory.Command => TypePath.New(typeof(ICommandHandler<,>).FullName!, [paramsType.FullName, resultType.FullName]),
                            _ => throw new NotSupportedException()
                        };
                        _ = handlerClass.AddBaseType(baseType)
                            .AddMember(new Field(fld(cmdPcr.Name), cmdPcr) { AccessModifier = IField.DefaultAccessModifier })
                            .AddMember(new Field(fld(qryPcr.Name), qryPcr) { AccessModifier = IField.DefaultAccessModifier })
                            .AddMember(new Field(fld(dal.Name), dal) { AccessModifier = IField.DefaultAccessModifier });
                        _ = handlerClass.AddMember(ctor);

                        // Create namespace
                        var ns = INamespace.New(model.CqrsNameSpace)
                            .AddUsingNameSpace(cmdPcr.GetNameSpaces())
                            .AddUsingNameSpace(qryPcr.GetNameSpaces())
                            .AddUsingNameSpace(dal.GetNameSpaces())
                            .AddUsingNameSpace(paramsType.GetNameSpaces())
                            .AddUsingNameSpace(resultType.GetNameSpaces())
                            .AddUsingNameSpace(baseType.GetNameSpaces())
                            .AddType(handlerClass);

                        var codeStatement = this._generatorEngine.Generate(ns);
                        var code = Code.New(handlerClass.Name, Languages.CSharp, codeStatement, true).SetCategory(category);
                        return code;
                    }
                    Code createParams(CqrsViewModelBase mode)
                    {
                        var segregateType = mode.GetSegregateParamsType(null);
                        var props = mode.ParamsDto.Properties
                            .Select(x => (Name: x.Name!, Type: x.IsList is true ? TypePath.New(typeof(List<>).FullName!, [x.TypeFullName]) : TypePath.New(x.TypeFullName))).ToList();
                        var segregateClass = new Class(segregateType.Name)
                        {
                            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                        };
                        props.ForEach(x => segregateClass.AddProperty(x.Name, x.Type));

                        var ctor = new Method(segregateType.Name) { IsConstructor = true };
                        props.ForEach(x => ctor.AddArgument(x.Type, arg(x.Name)));
                        var body = new StringBuilder();
                        props.ForEach(x => body.AppendLine($"this.{x.Name} = {arg(x.Name)};"));
                        ctor.Body = body.ToString();
                        segregateClass.AddMember(ctor);

                        var defCtor = new Method(segregateType.Name) { IsConstructor = true };
                        segregateClass.AddMember(defCtor);

                        var usings = segregateType.GetNameSpaces()
                            .AddRangeImmuted(props.Select(x => x.Type.GetNameSpaces()).SelectAll())
                            .Except([segregateType.NameSpace]);

                        var ns = INamespace.New(segregateType.NameSpace)
                            .AddUsingNameSpace(usings)
                        .AddType(segregateClass);

                        var codeStatement = this._generatorEngine.Generate(ns);
                        var code = Code.New(segregateType.Name, Languages.CSharp, codeStatement, true).SetCategory(CodeCategory.Dto);
                        return code;
                    }
                    Code createResult(CqrsViewModelBase mode)
                    {
                        var segregateType = mode.GetSegregateResultParamsType(null);
                        var props = mode.ResultDto.Properties
                            .Select(x => (Name: x.Name!, Type: x.IsList is true ? TypePath.New(typeof(List<>).FullName!, [x.TypeFullName]) : TypePath.New(x.TypeFullName))).ToList();
                        var segregateClass = new Class(segregateType.Name)
                        {
                            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                        };
                        props.ForEach(x => segregateClass.AddProperty(x.Name, x.Type));

                        var ctor = new Method(segregateType.Name) { IsConstructor = true };
                        props.ForEach(x => ctor.AddArgument(x.Type, arg(x.Name)));
                        var body = new StringBuilder();
                        props.ForEach(x => body.AppendLine($"this.{x.Name} = {arg(x.Name)};"));
                        ctor.Body = body.ToString();
                        segregateClass.AddMember(ctor);

                        var defCtor = new Method(segregateType.Name) { IsConstructor = true };
                        segregateClass.AddMember(defCtor);

                        var usings = segregateType.GetNameSpaces()
                            .AddRangeImmuted(props.Select(x => x.Type.GetNameSpaces()).SelectAll())
                            .Except([segregateType.NameSpace]);

                        var ns = INamespace.New(segregateType.NameSpace)
                            .AddUsingNameSpace(usings)
                        .AddType(segregateClass);

                        var codeStatement = this._generatorEngine.Generate(ns);
                        var code = Code.New(segregateType.Name, Languages.CSharp, codeStatement, true).SetCategory(CodeCategory.Dto);
                        return code;
                    }
                }
            }
        }
        static string arg(in string name)
            => TypeMemberNameHelper.ToArgName(name);
        static string fld(in string name)
            => TypeMemberNameHelper.ToFieldName(name);
        static string prp(in string name)
            => TypeMemberNameHelper.ToPropName(name);
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