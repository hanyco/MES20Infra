using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Data.SqlServer;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

using Services.Helpers;

using static Services.Helpers.CommonHelpers;

using ICommand = Library.Cqrs.Models.Commands.ICommand;

namespace Services;

[Service]
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
            yield return toCode(model.Name, "QueryParams", createParams(model, kind), false, CodeCategory.Dto);

            Result<string> createHandler(in CqrsViewModelBase model, CodeCategory kind)
            {
                var handlerBody = model.HandleMethodBody ?? $"return Task.FromResult<{model.GetResultType(kind.ToString()).Name}>(null!);";

                // Create `HandleAsync` method
                var handleAsyncMethod = new Method(nameof(IQueryHandler<string, string>.HandleAsync))
                {
                    AccessModifier = AccessModifier.Public,
                    Body = handlerBody,
                    Parameters =
                    {
                        (model.GetParamsType(kind.ToString()), kind.ToString().ToLower())
                    },
                    ReturnType = TypePath.New($"{typeof(Task<>).FullName}<{model.GetResultType(kind.ToString()).FullPath}>")
                };
                // Create `QueryHandler` class
                var handlerClass = new Class(model.GetHandlerClass(kind.ToString()))
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };
                _ = handlerClass.Members.Add(handleAsyncMethod);

                // Create namespace
                var ns = INamespace.New(model.CqrsNameSpace);
                _ = ns.Types.Add(handlerClass);

                // Generate result
                var result = codeGeneratorEngine.Generate(ns);
                return result;
            }

            Result<string> createParams(CqrsViewModelBase model, CodeCategory kind)
            {
                var dtoCodeResult = dtoCodeService.GenerateCodes(model.ParamsDto, new(model.GetParamsParam(kind.ToString()).Name));
                return dtoCodeResult.IsFailure
                    ? dtoCodeResult.WithValue(string.Empty)
                    : (Result<string>)dtoCodeResult.GetValue().Single()!.Statement;
            }
        }

        IEnumerable<Code> generatePartCode(CqrsViewModelBase model, CodeCategory kind)
        {
            yield return toCode(model.Name, "Handler", createQueryHandler(model, kind), true, kind);
            yield return toCode(model.Name, null, createSegregationType(model, kind), true, CodeCategory.Dto);
            yield return toCode(model.Name, "Result", createResult(model, kind), true, CodeCategory.Dto);

            Result<string> createQueryHandler(CqrsViewModelBase model, CodeCategory kind)
            {
                // Initialize
                var cmdPcr = TypePath.New(typeof(ICommandProcessor));
                var qryPcr = TypePath.New(typeof(IQueryProcessor));
                var dal = TypePath.New(typeof(Sql));

                // Create constructor
                var ctor = new Method(model.Name!)
                {
                    IsConstructor = true,
                    Body = @$"
                        (this.{fld(cmdPcr.Name)}, this.{fld(qryPcr.Name)}) = ({arg(cmdPcr.Name)}, {arg(qryPcr.Name)});
                        this.{fld(dal.Name)} = {arg(dal.Name)};
                        ",
                };
                _ = ctor.AddParameter(cmdPcr.Name, arg(cmdPcr.Name))
                    .AddParameter(qryPcr.Name, arg(qryPcr.Name))
                    .AddParameter(dal.Name, arg(dal.Name));

                // Create `QueryHandler` class
                var type = new Class(model.GetHandlerClass(kind.ToString()))
                {
                    AccessModifier = AccessModifier.Public,
                    InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
                };
                var paramsType = model.GetParamsType(kind.ToString());
                var resultType = model.GetResultType(kind.ToString());
                var baseType = kind switch
                {
                    CodeCategory.Query => TypePath.New(typeof(IQueryHandler<,>).FullName!, [paramsType.FullPath, resultType.FullPath]),
                    CodeCategory.Command => TypePath.New(typeof(ICommandHandler<,>).FullName!, [paramsType.FullPath, resultType.FullPath]),
                    _ => throw new NotImplementedException()
                };
                _ = type.BaseTypes.Add(baseType);
                _ = type.AddMember(new Field(fld(cmdPcr.Name), cmdPcr) { IsReadOnly = true })
                    .AddMember(new Field(fld(qryPcr.Name), qryPcr) { IsReadOnly = true })
                    .AddMember(new Field(fld(dal.Name), dal) { IsReadOnly = true });
                _ = type.AddMember(ctor);

                // Create namespace
                var ns = INamespace.New(model.CqrsNameSpace)
                    .AddType(type);

                // Generate code
                var result = codeGeneratorEngine.Generate(ns);
                return result;
            }

            Result<string> createResult(CqrsViewModelBase mode, CodeCategory kind)
            {
                var resultDto = mode.ResultDto;
                var className = resultDto.GetSegregateClassName(kind.ToString(), "Result");
                var paramsPropType = TypePath.New(resultDto.IsList
                    ? $"List<{Purify(resultDto.Name)}Result>"
                    : $"{Purify(resultDto.Name)}Result");

                var prop = new CodeGenProperty($"{prp("Result")}", paramsPropType);
                var ctor = new Method(className)
                {
                    IsConstructor = true,
                    Body = $"this.{prop.Name} = {arg(prop.Name)};",
                    Parameters =
                    {
                        (paramsPropType, arg(prop.Name))
                    }
                };
                var type = new Class(className).AddMember(ctor, prop);
                var nameSpace = INamespace.New(resultDto.NameSpace)
                    .AddType(type);

                // Generate code
                return codeGeneratorEngine.Generate(nameSpace);
            }

            Result<string> createSegregationType(CqrsViewModelBase model, CodeCategory kind)
            {
                var paramsDto = model.ParamsDto; // ex. GetByIdPerson
                var segregateName = paramsDto.GetSegregateClassName(kind.ToString()); // ex. GetByIdPersonQuery
                var paramsType = TypePath.New(paramsDto.IsList
                    ? $"List<{model.GetParamsParam(kind.ToString())}>"
                    : model.GetParamsParam(kind.ToString())); // ex. GetByIdPersonParams

                var paramsProp = new CodeGenProperty(prp("Params"), paramsType);
                var ctor = new Method(segregateName)
                {
                    IsConstructor = true,
                    Body = $"this.{paramsProp.Name} = {arg(paramsProp.Name)};",
                    Parameters =
                    {
                        (paramsType, arg(paramsProp.Name)) // ex. ({GetAllPeopleParams}, "@params")
                    }
                };
                var segregateType = new Class(segregateName)
                {
                    BaseTypes =
                    {
                        kind switch
                        {
                            CodeCategory.Query => TypePath.New(typeof(IQuery<>), [model.ResultDto.GetSegregateClassName(kind.ToString(), "Result")]),
                            CodeCategory.Command => TypePath.New<ICommand>(),
                            _ => throw new NotImplementedException() },
                    }
                }.AddMember(ctor).AddMember(paramsProp);
                var nameSpace = INamespace.New(paramsDto.NameSpace)
                    .AddType(segregateType);

                // Generate code
                return codeGeneratorEngine.Generate(nameSpace);
            }
        }
    }
}