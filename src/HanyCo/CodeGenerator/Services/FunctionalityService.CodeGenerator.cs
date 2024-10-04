using HanyCo.Infra.CodeGen.Contracts.CodeGen.Services;
using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2.Back;
using Library.Data.SqlServer;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

using MediatR;

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
        var cats = result.Value!.Select(x => x!.GetCategory());
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
                var codeGenRes = this.GenerateCqrsCodes(viewModel.GetAllQuery);
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
                var codeGenRes = this.GenerateCqrsCodes(viewModel.GetByIdQuery);
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
                var codeGenRes = this.GenerateCqrsCodes(viewModel.InsertCommand);
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
                var codeGenRes = this.GenerateCqrsCodes(viewModel.UpdateCommand);
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
                var codeGenRes = this.GenerateCqrsCodes(viewModel.DeleteCommand);
                codes.DeleteCommandCodes = codeGenRes.Select(x => x.Value).ToCodes();
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.DeleteCommand)}");
                yield return codes.DeleteCommandCodes;
                if (codeGenRes.Any(x => !x.IsSucceed))
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListPage != null)
            {
                var codeGenRes = this._blazorPageCodeService.GenerateCodes(viewModel.BlazorListPage);
                codes.BlazorListPageCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorListPage)}");
                yield return codes.BlazorListPageCodes!;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListPage?.DataContext != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.BlazorListPage.DataContext);
                codes.BlazorListPageDataContextCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorListPage.DataContext)}");
                yield return codes.BlazorListPageDataContextCodes!;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorDetailsPage != null)
            {
                var codeGenRes = this._blazorPageCodeService.GenerateCodes(viewModel.BlazorDetailsPage);
                codes.BlazorDetailsPageCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorDetailsPage)}");
                yield return codes.BlazorDetailsPageCodes!;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorDetailsPage?.DataContext != null)
            {
                var codeGenRes = this._dtoCodeService.GenerateCodes(viewModel.BlazorDetailsPage.DataContext);
                codes.BlazorListPageDataContextCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorDetailsPage.DataContext)}");
                yield return codes.BlazorListPageDataContextCodes!;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorListComponent != null)
            {
                var codeGenRes = this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorListComponent);
                codes.BlazorListComponentCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorListComponent)}");
                yield return codes.BlazorListComponentCodes!;
                if (!codeGenRes.IsSucceed)
                {
                    yield break;
                }
            }

            if (viewModel.BlazorDetailsComponent != null)
            {
                var editForm = viewModel.BlazorDetailsComponent.EditFormInfo;
                var args = new GenerateCodesParameters(
                    IsEditForm: editForm.IsEditForm,
                    EditFormAttributes: editForm.Events.Select(x => (x.Name, x.Handler.Name)).AddImmuted(("Model", editForm.Model))
                    );
                var codeGenRes = this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorDetailsComponent, args);
                codes.BlazorDetailsComponentCodes = codeGenRes;
                this._reporter.Report(max, ++index, $"Code generated for {nameof(viewModel.BlazorDetailsComponent)}");
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

    private static string arg(in string name) => TypeMemberNameHelper.ToArgName(name);

    private static string fld(in string name) => TypeMemberNameHelper.ToFieldName(name);

    private Code CreateMainHandler(CqrsViewModelBase model)
    {
        var handlerType = model.GetSegregateHandlerType();
        // Detect the segregation type, using type pattern matching
        var category = model switch
        {
            CqrsQueryViewModel => CodeCategory.Query,
            CqrsCommandViewModel => CodeCategory.Command,
            _ => throw new NotSupportedException()
        };

        var segregateClass = new Class(handlerType.Name)
        {
            AccessModifier = AccessModifier.Internal,
            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
        };

        var ns = INamespace.New(handlerType.NameSpace).AddType(segregateClass);

        var codeStatement = this._generatorEngine.Generate(ns);
        var code = Code.New(handlerType.Name, Languages.CSharp, codeStatement).SetCategory(category);
        return code;
    }

    private Code CreateMainParams(in CqrsViewModelBase model)
    {
        // Create segregation TypePath
        var segregateType = model.GetSegregateParamsType(null);

        // Create Command/Query class
        var segregateClass = new Class(segregateType.Name)
        {
            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
        };

        // Gather everything together
        var ns = INamespace.New(segregateType.NameSpace).AddType(segregateClass);

        // Generate code and result it.
        var codeStatement = this._generatorEngine.Generate(ns);
        var code = Code.New(segregateType.Name, Languages.CSharp, codeStatement).SetCategory(CodeCategory.Dto);
        return code;
    }

    private Code CreateMainResult(in CqrsViewModelBase model)
    {
        var segregateType = model.GetSegregateResultParamsType(null);
        var segregateClass = new Class(segregateType.Name)
        {
            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
        };

        var ns = INamespace.New(segregateType.NameSpace).AddType(segregateClass);

        var codeStatement = this._generatorEngine.Generate(ns);
        var code = Code.New(segregateType.Name, Languages.CSharp, codeStatement).SetCategory(CodeCategory.Dto);
        return code;
    }

    private Code CreatePartHandler(in CqrsViewModelBase model)
    {
        // Detect the segregation type, using type pattern matching
        var category = model switch
        {
            CqrsQueryViewModel => CodeCategory.Query,
            CqrsCommandViewModel => CodeCategory.Command,
            _ => throw new NotSupportedException()
        };

        // Create Handler TypePath
        var handlerType = model.GetSegregateHandlerType();

        // Create Handler class
        var handlerClass = new Class(handlerType.Name)
        {
            AccessModifier = AccessModifier.Internal,
            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
        };

        // Create handler's required fields
        var qryPcr = TypePath.New<IMediator>();
        var dal = TypePath.New<Sql>();

        // Create ctor
        var ctor = new Method(handlerType.Name)
        {
            IsConstructor = true,
            Arguments =
            {
                new(qryPcr, arg(qryPcr.Name)),
                new(dal, arg(dal.Name))
            },
            Body = $"""
                this.{fld(qryPcr.Name)} = {arg(qryPcr.Name)};
                this.{fld(dal.Name)} = {arg(dal.Name)};
                """
        };

        /// Initialize implementations
        var paramsType = model.GetSegregateParamsType(null!);
        var resultType = model.GetSegregateResultParamsType(null!);
        var baseType = TypePath.New(typeof(IRequestHandler<,>).FullName!, [paramsType.FullName, resultType.FullName]);

        // Add required fields and add implementation
        _ = handlerClass.AddBaseType(baseType)
            .AddMember(new Field(fld(qryPcr.Name), qryPcr) { AccessModifier = IField.DefaultAccessModifier })
            .AddMember(new Field(fld(dal.Name), dal) { AccessModifier = IField.DefaultAccessModifier });
        _ = handlerClass.AddMember(ctor);

        var handleMethod = new Method("Handle") { 
            AccessModifier = AccessModifier.Public,
            Body = model.HandleMethodBody??"throw new NotImplementedException();",
            IsAsync = true,
            ReturnType = TypePath.NewTask(model.ResultDto.Name!)
        }
        .AddArgument(model.ParamsDto.Name!, "request")
        .AddArgument(typeof(CancellationToken).Name, "cancellationToken");


        handlerClass.AddMember(handleMethod);

        // Gather and add `using`s
        var usings = handlerType.GetNameSpaces()
            .AddRangeImmuted(qryPcr.GetNameSpaces())
            .AddRangeImmuted(dal.GetNameSpaces())
            .AddRangeImmuted(paramsType.GetNameSpaces())
            .AddRangeImmuted(resultType.GetNameSpaces())
            .AddRangeImmuted(baseType.GetNameSpaces())
            .Except([model.CqrsNameSpace]);
        var ns = INamespace.New(model.CqrsNameSpace)
            .AddType(handlerClass)
            .AddUsingNameSpace(usings);

        var codeStatement = this._generatorEngine.Generate(ns);
        var code = Code.New(handlerClass.Name, Languages.CSharp, codeStatement, true).SetCategory(category);
        return code;
    }

    private Code CreatePartParams(in CqrsViewModelBase model)
    {
        // Create segregation TypePath
        var segregateType = model.GetSegregateParamsType(null);

        // Create Command/Query class
        var baseType = TypePath.New(typeof(IRequest<>).FullName!, [model.ResultDto.FullName]);
        var segregateClass = new Class(segregateType.Name)
        {
            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
        }.AddBaseType(baseType);

        // Add default ctor
        var defCtor = new Method(segregateType.Name)
        {
            IsConstructor = true,
            Body = "// Default constructor",
        };
        segregateClass.AddMember(defCtor);

        //Find properties
        var props = model.ParamsDto.Properties.Select(x => (Name: x.Name!, Type: TypePath.New(x.TypeFullName)));
        // Any property?
        if (props.Any()) // So create a proper ctor to initialize them.
        {
            // Add properties to Command/Query class
            props.ForEach(x => segregateClass.AddProperty(x.Name, x.Type));

            // Create ctor and initialize these properties
            var ctor = new Method(segregateType.Name) { IsConstructor = true };
            props.ForEach(x => ctor.AddArgument(x.Type, arg(x.Name)));
            var body = new StringBuilder();
            props.ForEach(x => body.AppendLine($"this.{x.Name} = {arg(x.Name)};"));
            ctor.Body = body.ToString();

            // Add ctor
            _ = segregateClass.AddMember(ctor);
        }

        // Gather `using` namespaces
        var usings = segregateType.GetNameSpaces()
            .AddRangeImmuted(segregateClass.BaseTypes.Select(x => x.GetNameSpaces()).SelectAll())
            .AddRangeImmuted(props.Select(x => x.Type.GetNameSpaces()).SelectAll())
            .Except([segregateType.NameSpace]);

        // Gather everything together
        var ns = INamespace.New(segregateType.NameSpace)
            .AddUsingNameSpace(usings)
            .AddType(segregateClass);

        // Generate code and result it.
        var codeStatement = this._generatorEngine.Generate(ns);
        var code = Code.New(segregateType.Name, Languages.CSharp, codeStatement, true).SetCategory(CodeCategory.Dto);
        return code;
    }

    private Code CreatePartResult(in CqrsViewModelBase mode)
    {
        var segregateType = mode.GetSegregateResultParamsType(null);
        var segregateClass = new Class(segregateType.Name)
        {
            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
        };

        var props = mode.ResultDto.Properties.Select(x => (Name: x.Name!, Type: x.IsList is true ? TypePath.New(typeof(List<>).FullName!, [x.TypeFullName]) : TypePath.New(x.TypeFullName))).ToList();
        if (props.Any())
        {
            props.ForEach(x => segregateClass.AddProperty(x.Name, x.Type));

            var ctor = new Method(segregateType.Name) { IsConstructor = true };
            props.ForEach(x => ctor.AddArgument(x.Type, arg(x.Name)));
            var body = new StringBuilder();
            props.ForEach(x => body.AppendLine($"this.{x.Name} = {arg(x.Name)};"));
            ctor.Body = body.ToString();
            _ = segregateClass.AddMember(ctor);
        }

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

    private ImmutableArray<Result<Codes>> GenerateCqrsCodes(in CqrsViewModelBase cqrsViewModel)
    {
        return gather(cqrsViewModel).ToImmutableArray();

        IEnumerable<Result<Codes>> gather(CqrsViewModelBase model)
        {
            var partParamsDtoCode = this.CreatePartParams(model);
            var mainParamsDtoCode = this.CreateMainParams(model);
            var partResultDtoCode = this.CreatePartResult(model);
            var mainResultDtoCode = this.CreateMainResult(model);
            var partHandlersCode = this.CreatePartHandler(model);
            var mainHandlersCode = this.CreateMainHandler(model);

            yield return SourceCodeHelpers.ToCodes(mainParamsDtoCode, partParamsDtoCode);
            yield return SourceCodeHelpers.ToCodes(mainResultDtoCode, partResultDtoCode);
            yield return SourceCodeHelpers.ToCodes(mainHandlersCode, partHandlersCode);
        }
    }
}