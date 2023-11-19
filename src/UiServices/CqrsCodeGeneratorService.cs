using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.Markers;
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
internal sealed class CqrsCodeGeneratorService(ICodeGeneratorEngine codeGeneratorEngine) : ICqrsCodeGeneratorService
{
    public Result<Codes> GenerateCodes(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null)
    {
        var result = new Result<Codes>(viewModel.ArgumentNotNull() switch
        {
            CqrsQueryViewModel queryViewModel => this.GenerateQuery(queryViewModel),
            CqrsCommandViewModel commandViewModel => this.GenerateCommand(commandViewModel),
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

    private static Code toCode(in string? modelName, in string codeName, in Result<string> statement, bool isPartial, CodeCategory codeCategory) =>
        Code.New($"{modelName}{codeName}", Languages.CSharp, statement, isPartial).With(x => x.props().Category = codeCategory);

    private Codes GenerateCommand(in CqrsCommandViewModel model)
    {
        return this.GenerateSegregation(model, CodeCategory.Command);
        ;
        //Check.MustBeArgumentNotNull(model?.Name);

        //var mainCodes = generateMainCode(model);
        //var partCodes = generatePartCode(model);

        //return Codes.New(mainCodes.ToCodes(), partCodes.ToCodes());

        //IEnumerable<Code> generateMainCode(CqrsCommandViewModel model)
        //{
        //    var statement = createCommandHandler(model);
        //    yield return toCode(model.Name, "Handler", statement, false, CodeCategory.Command);

        // Result<string> createCommandHandler(CqrsCommandViewModel model) { var handlerBody =
        // model.HandleMethodBody ?? $"return Task.FromResult<{model.GetResultType("Command").Name}>(null!);";

        // // Create `HandleAsync` method var handleAsyncMethod = new
        // Method(nameof(ICommandHandler<ICommand, string>.HandleAsync)) { AccessModifier =
        // AccessModifier.Public, Body = handlerBody, ReturnType =
        // TypePath.New($"{typeof(Task<>).FullName}<{model.GetResultType("Command").FullPath}>") };
        // //x (model.GetParamsType("Command"), "command") _ = handleAsyncMethod.Parameters.AddRange(model.HandleMethodParameters);

        // // Create `CommandHandler` class var handlerClass = new
        // Class(model.GetHandlerClass("Command")) { AccessModifier = AccessModifier.Public,
        // InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial }; _ = handlerClass.Members.Add(handleAsyncMethod);

        // // Create namespace var ns = INamespace.New(model.CqrsNameSpace); _ = ns.Types.Add(handlerClass);

        //        // Generate result
        //        var result = codeGeneratorEngine.Generate(ns);
        //        return result;
        //    }
        //}

        //IEnumerable<Code> generatePartCode(CqrsCommandViewModel model)
        //{
        //    yield return toCode(model.Name, "Handler", createCommandHandler(model), true, CodeCategory.Command);
        //    yield return toCode(model.Name, "Params", createCommandParams(model), true, CodeCategory.Dto);
        //    yield return toCode(model.Name, "Result", createCommandResult(model), true, CodeCategory.Dto);

        // Result<string> createCommandParams(CqrsCommandViewModel model) { var paramsDto =
        // model.ParamsDto; var className = paramsDto.GetSegregateClassName("Command", null); var
        // paramsPropType = TypePath.New(paramsDto.IsList ?
        // $"IEnumerable<{paramsDto.GetSegregateClassName(null, "Params")}>" :
        // paramsDto.GetSegregateClassName(null, "Params"));

        // var prop = new CodeGenProperty(prp("Params"), paramsPropType); var ctor = new
        // Method(className) { IsConstructor = true, Body = $"this.{prop.Name} = {arg(prop.Name)};",
        // Parameters = { (paramsPropType, arg(prop.Name)) } }; var type = new Class(className) {
        // BaseTypes = { TypePath.New(typeof(ICommand)), } }.AddMember(ctor, prop); var nameSpace =
        // INamespace.New(paramsDto.NameSpace) .AddType(type);

        // // Generate code return codeGeneratorEngine.Generate(nameSpace); }

        // Result<string> createCommandHandler(CqrsCommandViewModel model) { // Initialize var
        // cmdPcr = TypePath.New(typeof(ICommandProcessor)); var qryPcr =
        // TypePath.New(typeof(IQueryProcessor)); var dal = TypePath.New(typeof(Sql));

        // // Create constructor var ctor = new Method(model.Name!) { IsConstructor = true, Body =
        // @$" (this.{fld(cmdPcr.Name)}, this.{fld(qryPcr.Name)}) = ({arg(cmdPcr.Name)},
        // {arg(qryPcr.Name)}); this.{fld(dal.Name)} = {arg(dal.Name)}; ", }
        // .AddParameter(cmdPcr.Name, arg(cmdPcr.Name)) .AddParameter(qryPcr.Name, arg(qryPcr.Name))
        // .AddParameter(dal.Name, arg(dal.Name));

        // // Create `CommandHandler` class var paramsType = model.GetParamsType("Command"); var
        // resultType = model.GetResultType("Command"); var baseType =
        // TypePath.New($"{typeof(ICommandHandler<,>).FullName}", new[] { paramsType.FullPath,
        // resultType.FullPath }); var type = new Class(model.GetHandlerClass("Command")) {
        // AccessModifier = AccessModifier.Public, InheritanceModifier = InheritanceModifier.Sealed
        // | InheritanceModifier.Partial } .AddBaseType(baseType) .AddMember(new
        // Field(fld(cmdPcr.Name), cmdPcr) { IsReadOnly = true }) .AddMember(new
        // Field(fld(qryPcr.Name), qryPcr) { IsReadOnly = true }) .AddMember(new
        // Field(fld(dal.Name), dal) { IsReadOnly = true }) .AddMember(ctor);

        // // Create namespace var ns = INamespace.New(model.CqrsNameSpace); _ = ns.Types.Add(type);

        // // Generate code var result = codeGeneratorEngine.Generate(ns); return result; }

        // Result<string> createCommandResult(CqrsCommandViewModel mode) { var resultDto =
        // mode.ResultDto; var className = resultDto.GetSegregateClassName("Command", "Result"); var
        // paramsPropType = TypePath.New(resultDto.IsList ? $"List<{Purify(resultDto.Name)}Result>"
        // : $"{Purify(resultDto.Name)}Result");

        // var prop = new CodeGenProperty($"{prp("Result")}", paramsPropType); var ctor = new
        // Method(className) { IsConstructor = true, Body = $"this.{prop.Name} = {arg(prop.Name)};",
        // Parameters = { (paramsPropType, arg(prop.Name)) } }; var type = new
        // Class(className).AddMember(ctor, prop); var nameSpace =
        // INamespace.New(resultDto.NameSpace); _ = nameSpace.Types.Add(type);

        //        // Generate code
        //        return codeGeneratorEngine.Generate(nameSpace);
        //    }
        //}
    }

    private Codes GenerateQuery(in CqrsQueryViewModel model)
    {
        return this.GenerateSegregation(model, CodeCategory.Query);
        ;

        //Check.MustBeArgumentNotNull(model?.Name);

        //var partCodes = generatePartCode(model);
        //var mainCodes = generateMainCode(model);

        //return Codes.New(mainCodes.ToCodes(), partCodes.ToCodes());

        //IEnumerable<Code> generateMainCode(CqrsQueryViewModel model)
        //{
        //    var statement = createQueryHandler(model);
        //    yield return toCode(model.Name, "Handler", statement, false, CodeCategory.Query);

        //    Result<string> createQueryHandler(in CqrsQueryViewModel model)
        //    {
        //        var handlerBody = model.HandleMethodBody ?? $"return Task.FromResult<{model.GetResultType("Query").Name}>(null!);";

        //        // Create `HandleAsync` method
        //        var handleAsyncMethod = new Method(nameof(IQueryHandler<string, string>.HandleAsync))
        //        {
        //            AccessModifier = AccessModifier.Public,
        //            Body = handlerBody,
        //            Parameters =
        //            {
        //                (model.GetParamsType("Query"), "query")
        //            },
        //            ReturnType = TypePath.New($"{typeof(Task<>).FullName}<{model.GetResultType("Query").FullPath}>")
        //        };
        //        // Create `QueryHandler` class
        //        var handlerClass = new Class(model.GetHandlerClass("Query"))
        //        {
        //            AccessModifier = AccessModifier.Public,
        //            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
        //        };
        //        _ = handlerClass.Members.Add(handleAsyncMethod);

        //        // Create namespace
        //        var ns = INamespace.New(model.CqrsNameSpace);
        //        _ = ns.Types.Add(handlerClass);

        //        // Generate result
        //        var result = codeGeneratorEngine.Generate(ns);
        //        return result;
        //    }
        //}

        //IEnumerable<Code> generatePartCode(CqrsQueryViewModel model)
        //{
        //    yield return toCode(model.Name, "Handler", createQueryHandler(model), true, CodeCategory.Query);
        //    yield return toCode(model.Name, "Params", createQueryParams(model), true, CodeCategory.Dto);
        //    yield return toCode(model.Name, "Result", createQueryResult(model), true, CodeCategory.Dto);

        //    Result<string> createQueryHandler(CqrsQueryViewModel model)
        //    {
        //        // Initialize
        //        var cmdPcr = TypePath.New(typeof(ICommandProcessor));
        //        var qryPcr = TypePath.New(typeof(IQueryProcessor));
        //        var dal = TypePath.New(typeof(Sql));

        //        // Create constructor
        //        var ctor = new Method(model.Name!)
        //        {
        //            IsConstructor = true,
        //            Body = @$"
        //                (this.{fld(cmdPcr.Name)}, this.{fld(qryPcr.Name)}) = ({arg(cmdPcr.Name)}, {arg(qryPcr.Name)});
        //                this.{fld(dal.Name)} = {arg(dal.Name)};
        //                ",
        //        };
        //        _ = ctor.AddParameter(cmdPcr.Name, arg(cmdPcr.Name));
        //        _ = ctor.AddParameter(qryPcr.Name, arg(qryPcr.Name));
        //        _ = ctor.AddParameter(dal.Name, arg(dal.Name));

        //        // Create `QueryHandler` class
        //        var type = new Class(model.GetHandlerClass("Query"))
        //        {
        //            AccessModifier = AccessModifier.Public,
        //            InheritanceModifier = InheritanceModifier.Sealed | InheritanceModifier.Partial
        //        };
        //        var paramsType = model.GetParamsType("Query");
        //        var resultType = model.GetResultType("Query");
        //        var baseType = TypePath.New($"{typeof(IQueryHandler<,>).FullName}", new[] { paramsType.FullPath, resultType.FullPath });
        //        _ = type.BaseTypes.Add(baseType);
        //        _ = type.AddMember(new Field(fld(cmdPcr.Name), cmdPcr) { IsReadOnly = true });
        //        _ = type.AddMember(new Field(fld(qryPcr.Name), qryPcr) { IsReadOnly = true });
        //        _ = type.AddMember(new Field(fld(dal.Name), dal) { IsReadOnly = true });
        //        _ = type.AddMember(ctor);

        //        // Create namespace
        //        var ns = INamespace.New(model.CqrsNameSpace);
        //        _ = ns.Types.Add(type);

        //        // Generate code
        //        var result = codeGeneratorEngine.Generate(ns);
        //        return result;
        //    }

        //    Result<string> createQueryResult(CqrsQueryViewModel mode)
        //    {
        //        var resultDto = mode.ResultDto;
        //        var className = resultDto.GetSegregateClassName("Query", "Result");
        //        var paramsPropType = TypePath.New(resultDto.IsList
        //            ? $"List<{Purify(resultDto.Name)}Result>"
        //            : $"{Purify(resultDto.Name)}Result");

        //        var prop = new CodeGenProperty($"{prp("Result")}", paramsPropType);
        //        var ctor = new Method(className)
        //        {
        //            IsConstructor = true,
        //            Body = $"this.{prop.Name} = {arg(prop.Name)};",
        //            Parameters =
        //            {
        //                (paramsPropType, arg(prop.Name))
        //            }
        //        };
        //        var type = new Class(className).AddMember(ctor, prop);
        //        var nameSpace = INamespace.New(resultDto.NameSpace);
        //        _ = nameSpace.Types.Add(type);

        //        // Generate code
        //        return codeGeneratorEngine.Generate(nameSpace);
        //    }

        //    Result<string> createQueryParams(CqrsQueryViewModel model)
        //    {
        //        var paramsDto = model.ParamsDto;
        //        var className = paramsDto.GetSegregateClassName("Query", "Params");
        //        var paramsPropType = TypePath.New(paramsDto.IsList
        //            ? $"IEnumerable<{paramsDto.GetSegregateClassName(null, "Params")}>"
        //            : paramsDto.GetSegregateClassName(null, "Params"));

        //        var prop = new CodeGenProperty(prp("Params"), paramsPropType);
        //        var ctor = new Method(className)
        //        {
        //            IsConstructor = true,
        //            Body = $"this.{prop.Name} = {arg(prop.Name)};",
        //            Parameters =
        //            {
        //                (paramsPropType, arg(prop.Name))
        //            }
        //        };
        //        var type = new Class(className)
        //        {
        //            BaseTypes =
        //            {
        //                TypePath.New(typeof(IQuery<>), [model.ResultDto.GetSegregateClassName("Query", "Result")]),
        //            }
        //        }.AddMember(ctor, prop);
        //        var nameSpace = INamespace.New(paramsDto.NameSpace)
        //            .AddType(type);

        //        // Generate code
        //        return codeGeneratorEngine.Generate(nameSpace);
        //    }
        //}
    }

    private Codes GenerateSegregation(in CqrsViewModelBase model, in CodeCategory kind)
    {
        Check.MustBeArgumentNotNull(model?.Name);

        var partCodes = generatePartCode(model, kind);
        var mainCodes = generateMainCode(model, kind);

        var allCodes = mainCodes.AddRangeImmuted(partCodes).OrderBy(x => x.Name);

        return allCodes.ToCodes();

        IEnumerable<Code> generateMainCode(CqrsViewModelBase model, CodeCategory kind)
        {
            yield return toCode(model.Name, "Handler", createHandler(model, kind), false, kind);
            yield return toCode(model.Name, "Parameters", createParams(model, kind), false, CodeCategory.Dto);

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

            Result<string> createParams(in CqrsViewModelBase model, CodeCategory kind)
            {
                var paramsClassTye = model.GetParamsParam(kind.ToString());
                var paramsClass = new Class(paramsClassTye.Name);
                var ns = INamespace.New(model.DtoNameSpace)
                    .AddType(paramsClass);
                // Generate result
                var result = codeGeneratorEngine.Generate(ns);
                return result;
            }
        }

        IEnumerable<Code> generatePartCode(CqrsViewModelBase model, CodeCategory kind)
        {
            yield return toCode(model.Name, "Handler.partial", createQueryHandler(model, kind), true, kind);
            yield return toCode(model.Name, "Params.partial", createParams(model, kind), true, CodeCategory.Dto);
            yield return toCode(model.Name, "Result.partial", createResult(model, kind), true, CodeCategory.Dto);

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
                    CodeCategory.Query => TypePath.New($"{typeof(IQueryHandler<,>).FullName}", new[] { paramsType.FullPath, resultType.FullPath }),
                    CodeCategory.Command => TypePath.New(typeof(ICommand)),
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

            Result<string> createParams(CqrsViewModelBase model, CodeCategory kind)
            {
                var paramsDto = model.ParamsDto;
                var className = paramsDto.GetSegregateClassName(kind.ToString(), "Params");
                var paramsPropType = TypePath.New(paramsDto.IsList
                    ? $"IEnumerable<{paramsDto.GetSegregateClassName()}>"
                    : paramsDto.GetSegregateClassName());

                var prop = new CodeGenProperty(prp("Params"), paramsPropType);
                var ctor = new Method(className)
                {
                    IsConstructor = true,
                    Body = $"this.{prop.Name} = {arg(prop.Name)};",
                    Parameters =
                    {
                        (paramsPropType, arg(prop.Name))
                    }
                };
                var type = new Class(className)
                {
                    BaseTypes =
                    {
                        TypePath.New(typeof(IQuery<>), [model.ResultDto.GetSegregateClassName(kind.ToString(), "Result")]),
                    }
                }.AddMember(ctor, prop);
                var nameSpace = INamespace.New(paramsDto.NameSpace)
                    .AddType(type);

                // Generate code
                return codeGeneratorEngine.Generate(nameSpace);
            }
        }
    }
}