
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.Markers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;
using Library.DesignPatterns.Markers;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

using Services.Helpers;

using ICommand = Library.Cqrs.Models.Commands.ICommand;

namespace Services;

[Service]
[Stateless]
internal sealed class CqrsCodeGeneratorService(ICodeGeneratorEngine codeGeneratorEngine, IDtoCodeService dtoCodeService) : ICqrsCodeGeneratorService
{
    public Result<Codes> GenerateCodes(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null)
    {
        var result = new Result<Codes>(viewModel.ArgumentNotNull() switch
        {
            CqrsQueryViewModel model => this.GenerateSegregation(model, CodeCategory.Query),
            CqrsCommandViewModel model => this.GenerateSegregation(model, CodeCategory.Command),
            _ => throw new NotSupportedException()
        });
        return result;
    }

    private static string arg(in string name) =>
        TypeMemberNameHelper.ToArgName(name);

    private static string fld(in string name) =>
        TypeMemberNameHelper.ToFieldName(name);

    private static string prp(in string name) =>
        TypeMemberNameHelper.ToPropName(name);

    private static Code toCode(in string? modelName, in string? codeName, in Result<string> statement, bool isPartial, CodeCategory codeCategory) =>
        Code.New($"{modelName}{codeName}", Languages.CSharp, statement, isPartial).With(x => x.props().Category = codeCategory);

    private Result<string> CreateValidator(in CqrsViewModelBase model)
    {
        var validatorMethod = new Method(nameof(ICommandValidator<ICommand>.ValidateAsync))
        {
            Body = model.ValidatorBody ?? "return ValueTask.CompletedTask;",
            ReturnType = TypePath.New<ValueTask>(),
        }.AddParameter(model.GetSegregateType("Command").Name, "command");

        var commandValidatorBaseType = TypePath.New(typeof(ICommandValidator<>), [model.GetSegregateType("Command").FullPath]);
        var validatorType = new Class(model.GetSegregateValidatorType("Command").Name)
            .AddBaseType(commandValidatorBaseType)
            .AddMember(validatorMethod);
        var ns = INamespace.New(model.CqrsNameSpace)
            .AddUsingNameSpace(model.DtoNameSpace)
            .AddUsingNameSpace(commandValidatorBaseType.GetNameSpaces())
            .AddUsingNameSpace(model.ValidatorAdditionalUsings)
            .AddType(validatorType);

        return codeGeneratorEngine.Generate(ns);
    }

    private Codes GenerateSegregation(in CqrsViewModelBase model, in CodeCategory kind)
    {
        //return Codes.Empty;
        Check.MustBeArgumentNotNull(model?.Name);

        var partCodes = generatePartCode(model, kind);
        //x return partCodes.ToCodes();
        var mainCodes = generateMainCode(model, kind);
        //x return mainCodes.ToCodes();

        return partCodes.AddRangeImmuted(mainCodes).OrderBy(x => x).ToCodes();

        IEnumerable<Code> generateMainCode(CqrsViewModelBase model, CodeCategory kind)
        {
            yield return toCode(model.Name, "Handler", createHandler(model, kind), false, kind);
            if (kind == CodeCategory.Command && model.ValidatorBody.IsNullOrEmpty())
            {
                yield return toCode(model.Name, "Validator", this.CreateValidator(model), false, kind);
            }

            Result<string> createHandler(in CqrsViewModelBase model, CodeCategory kind)
            {
                // Create `HandleAsync` method
                var handlerMethodBody = model.HandleMethodBody ?? "throw new NotImplementedException();";
                var handlerMethodName = kind switch
                {
                    CodeCategory.Query => nameof(IQueryHandler<string, string>.HandleAsync),
                    CodeCategory.Command => nameof(ICommandHandler<ICommand, string>.HandleAsync),
                    _ => throw new NotImplementedException(),
                };
                var handleAsyncMethod = new Method(handlerMethodName)
                {
                    AccessModifier = AccessModifier.Public,
                    Body = handlerMethodBody,
                    Parameters =
                    {
                        (model.GetSegregateType(kind.ToString()).FullPath, kind.ToString().ToLower())
                    },
                    ReturnType = TypePath.New($"{typeof(Task<>).FullName}<{model.GetSegregateResultType(kind.ToString()).FullPath}>")
                };

                // Create `QueryHandler` class
                var handlerClass = new Class(model.GetSegregateHandlerType(kind.ToString()))
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };
                _ = handlerClass.Members.Add(handleAsyncMethod);

                // Create namespace
                var ns = INamespace.New(model.CqrsNameSpace)
                    .AddUsingNameSpace(model.GetSegregateType(kind.ToString()).GetNameSpaces())
                    .AddUsingNameSpace(model.GetSegregateResultType(kind.ToString()).GetNameSpaces())
                    .AddType(handlerClass);

                // Generate result
                var result = codeGeneratorEngine.Generate(ns);
                return result;
            }
        }

        IEnumerable<Code> generatePartCode(CqrsViewModelBase model, CodeCategory kind)
        {
            if (kind == CodeCategory.Command && !model.ValidatorBody.IsNullOrEmpty())
            {
                yield return toCode(model.Name, "Validator", this.CreateValidator(model), true, kind);
            }

            yield return toCode(model.Name, "Handler", createQueryHandler(model, kind), true, kind);
            yield return toCode(model.Name, null, createSegregation(model, kind), true, CodeCategory.Dto);
            yield return toCode(model.Name, "Result", createResult(model, kind), true, CodeCategory.Dto);

            Result<string> createQueryHandler(CqrsViewModelBase model, CodeCategory kind)
            {
                // Initialize
                var cmdPcr = TypePath.New(typeof(ICommandProcessor));
                var qryPcr = TypePath.New(typeof(IQueryProcessor));
                var dal = TypePath.New(typeof(Sql));

                // Create constructor
                var ctor = new Method(model.GetSegregateHandlerType(kind.ToString()).Name)
                {
                    IsConstructor = true,
                    Parameters =
                    {
                        (cmdPcr, arg(cmdPcr.Name)),
                        (qryPcr, arg(qryPcr.Name)),
                        (dal, arg(dal.Name))
                    },
                    Body = @$"
                        (this.{fld(cmdPcr.Name)}, this.{fld(qryPcr.Name)}) = ({arg(cmdPcr.Name)}, {arg(qryPcr.Name)});
                        this.{fld(dal.Name)} = {arg(dal.Name)};
                        "
                };

                // Create `QueryHandler` class
                var handlerClass = new Class(model.GetSegregateHandlerType(kind.ToString()).Name)
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };

                // Add inheritance
                var paramsType = model.GetSegregateType(kind.ToString());
                var resultType = model.GetSegregateResultType(kind.ToString());
                var baseType = kind switch
                {
                    CodeCategory.Query => TypePath.New(typeof(IQueryHandler<,>).FullName!, [paramsType.FullPath, resultType.FullPath]),
                    CodeCategory.Command => TypePath.New(typeof(ICommandHandler<,>).FullName!, [paramsType.FullPath, resultType.FullPath]),
                    _ => throw new NotImplementedException()
                };

                // Add members
                _ = handlerClass.AddBaseType(baseType)
                    .AddMember(new Field(fld(cmdPcr.Name), cmdPcr) { IsReadOnly = true })
                    .AddMember(new Field(fld(qryPcr.Name), qryPcr) { IsReadOnly = true })
                    .AddMember(new Field(fld(dal.Name), dal) { IsReadOnly = true });
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

                // Generate code
                var result = codeGeneratorEngine.Generate(ns);
                return result;
            }

            Result<string> createResult(CqrsViewModelBase mode, CodeCategory kind)
            {
                var resultClassType = model.GetSegregateResultType(kind.ToString());
                var resultParamsType = TypePath.New(model.ResultDto.IsList
                    ? $"List<{model.GetSegregateResultParamsType(kind.ToString()).FullPath}>"
                    : $"{model.GetSegregateResultParamsType(kind.ToString()).FullPath}");

                var prop = new CodeGenProperty($"{prp("Result")}", resultParamsType);
                var ctor = new Method(resultClassType.Name)
                {
                    IsConstructor = true,
                    Body = $"this.{prop.Name} = {arg(prop.Name)};",
                    Parameters =
                    {
                        (prop.Type, arg(prop.Name))
                    }
                };
                var resultType = new Class(resultClassType.Name)
                    .AddMember(ctor)
                    .AddMember(prop);
                var nameSpace = INamespace.New(resultClassType.NameSpace)
                    .AddUsingNameSpace(prop.Type.GetNameSpaces())
                    .AddType(resultType);

                // Generate code
                return codeGeneratorEngine.Generate(nameSpace);
            }

            Result<string> createSegregation(CqrsViewModelBase model, CodeCategory kind)
            {
                var segregateType = model.GetSegregateType(kind.ToString()); // ex. GetByIdPersonQuery
                var paramsType = TypePath.New(model.ParamsDto.IsList
                    ? $"List<{model.GetSegregateParamsType(kind.ToString())}>"
                    : model.GetSegregateParamsType(kind.ToString())); // ex. GetByIdPersonParams

                var paramsProp = new CodeGenProperty(prp("Params"), paramsType);
                var ctor = new Method(segregateType)
                {
                    IsConstructor = true,
                    Body = $"this.{paramsProp.Name} = {arg(paramsProp.Name)};",
                    Parameters =
                    {
                        (paramsType, arg(paramsProp.Name)) // ex. ({GetAllPeopleParams}, "@params")
                    }
                };
                var segregateClass = new Class(segregateType.Name)
                {
                    BaseTypes =
                    {
                        kind switch
                        {
                            CodeCategory.Query => TypePath.New(typeof(IQuery<>), [model.GetSegregateResultType(kind.ToString())]),
                            CodeCategory.Command => TypePath.New<ICommand>(),
                            _ => throw new NotImplementedException() },
                    }
                }.AddMember(ctor).AddMember(paramsProp);
                var nameSpace = INamespace.New(segregateType.NameSpace)
                    .AddUsingNameSpace(paramsProp.Type.GetNameSpaces())
                    .AddType(segregateClass);

                // Generate code
                return codeGeneratorEngine.Generate(nameSpace);
            }
        }
    }
}