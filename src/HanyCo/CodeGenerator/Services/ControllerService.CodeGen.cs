using System.ComponentModel.Design;
using System.Text;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.DesignPatterns.Markers;
using Library.Helpers.CodeGen;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services;

[Service]
[Stateless]
internal sealed partial class ControllerService(
    InfraWriteDbContext writeDbContext,
    InfraReadDbContext readDbContext,
    ICodeGeneratorEngine codeGeneratorEngine,
    IEntityViewModelConverter converter,
    ISecurityService securityService) 
    : IControllerService
{
    public Result<Codes> GenerateCodes(ControllerViewModel viewModel)
    {
        // Validations
        var vr = viewModel
            .ArgumentNotNull().Check()
            .NotNull(x => x.Name).Build();
        if (vr.IsFailure)
        {
            return vr.WithValue(Codes.Empty)!;
        }

        // Create namespace
        var ns = INamespace.New(viewModel.NameSpace);

        // Create controller
        var controllerClass = createController(viewModel);

        // Add ctor to controller
        var ctor = createConstructor(viewModel, ns, controllerClass);
        _ = controllerClass.AddMember(ctor);

        // Add APIs
        foreach (var api in viewModel.Apis)
        {
            // Parse return type
            var returnType = processReturnType(ns, api);

            // Create method
            var method = createMethod(ns, api, returnType);

            // Add API to Controller
            _ = controllerClass.AddMember(method);
        }

        // Add Controller to Namespace
        _ = ns.AddType(controllerClass);

        // Add additional usings to namespace
        viewModel.AdditionalUsings.ForEach(x => ns.AddUsingNameSpace(x));

        // Generate code
        var codeStatement = codeGeneratorEngine.Generate(ns);
        if (codeStatement.IsFailure)
        {
            return codeStatement.WithValue(Codes.Empty)!;
        }

        var partCode = Code.New(viewModel.Name, Languages.CSharp, codeStatement, true).SetCategory(CodeCategory.Api);
        // TODO: Add main part to let the developer to add his/her own code to the controller.
        var mainCode = Code.New(viewModel.Name, Languages.CSharp, "// Working on it... To be back soon.", false).SetCategory(CodeCategory.Api);

        // Return result
        return Codes.Create(mainCode, partCode);

        static IClass createController(in ControllerViewModel viewModel)
        {
            var controllerClass = IClass
                .New(viewModel.Name)
                .AddBaseType<ControllerBase>()
                .AddAttribute<ApiControllerAttribute>()
                .AddAttribute<RouteAttribute>((null, "[controller]"));
            if (viewModel.IsAnonymousAllow)
            {
                controllerClass.UsingNamesSpaces.Add(typeof(AllowAnonymousAttribute).Namespace!);
                _ = controllerClass.AddAttribute<AllowAnonymousAttribute>();
            }

            return controllerClass;
        }

        static Method createConstructor(in ControllerViewModel viewModel, in INamespace ns, in IClass controllerClass)
        {
            var ctor = new Method(controllerClass.Name)
            {
                IsConstructor = true,
            };
            var ctorBody = new StringBuilder();
            foreach (var (argument, isField) in viewModel.CtorParams)
            {
                _ = ctor.AddArgument(argument);
                if (isField)
                {
                    var fieldName = TypeMemberNameHelper.ToFieldName(argument.Name);
                    _ = ns.AddUsingNameSpace(argument.Type.GetNameSpaces());
                    _ = controllerClass.AddField(fieldName, argument.Type);
                    _ = ctorBody.AppendLine($"this.{fieldName} = {argument.Name};");
                }
            }
            ctor.Body = ctorBody.ToString();
            return ctor;
        }

        static string processReturnType(in INamespace ns, in ControllerMethodViewModel api)
        {
            string returnType;
            if (api.ReturnType is not null)
            {
                returnType = api.ReturnType.AsKeyword();
                _ = ns.AddUsingNameSpace(api.ReturnType.GetNameSpaces());
            }
            else
            {
                returnType = "void";
            }

            return returnType;
        }

        static Method createMethod(in INamespace ns, in ControllerMethodViewModel api, in string returnType)
        {
            var method = new Method(api.Name!)
            {
                Body = api.Body,
                ReturnType = returnType,
                IsAsync = api.IsAsync,
            };
            api.Arguments.ForEach(x => method.AddArgument(x));

            // Add HTTP methods
            foreach (var httpMethod in api.HttpMethods)
            {
                var httpType = httpMethod.GetType();
                var route = httpType.GetProperty(nameof(httpMethod.Template))!.GetValue(httpMethod)?.ToString();
                _ = route.IsNullOrEmpty()
                    ? method.AddAttribute(TypePath.New(httpType))
                    : method.AddAttribute(TypePath.New(httpType), (null, route));
                _ = ns.AddUsingNameSpace(httpType.Namespace!);
            }

            return method;
        }
    }
}