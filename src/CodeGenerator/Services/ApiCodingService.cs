using System.Text;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.CodeGen;

internal sealed class ApiCodingService(ICodeGeneratorEngine codeGeneratorEngine) : IApiCodingService
{
    public Result<Codes?> GenerateCodes(ApiCodingViewModel viewModel, ApiCodingArgs? arguments = null)
    {
        // Validations
        _ = viewModel.ArgumentNotNull();

        var vr = viewModel.Check()
            .NotNull(x => x.ControllerName)
            .Build();

        if (vr.IsFailure)
        {
            return vr.WithValue(Codes.Empty)!;
        }

        // Create namespace
        var ns = INamespace.New(viewModel.NameSpace);

        // Create controller
        var controllerClass = IClass.New(viewModel.ControllerName)
            .AddBaseType(typeof(ControllerBase))
            .AddAttribute(typeof(ApiControllerAttribute))
            .AddAttribute(typeof(RouteAttribute), (null, "\"[controller]\""));
        if (viewModel.IsAnonymousAllow)
        {
            _ = controllerClass.AddAttribute(typeof(AllowAnonymousAttribute));
        }

        // Add ctor to controller
        var ctor = new Method(controllerClass.Name)
        {
            IsConstructor = true,
        };
        var ctorBody = new StringBuilder();

        foreach (var (Argument, IsField) in viewModel.CtorParams)
        {
            _ = ctor.Arguments.Add(Argument);
            if (IsField)
            {
                var fieldName = TypeMemberNameHelper.ToFieldName(Argument.Name);
                _ = controllerClass.AddField(fieldName, Argument.Type);
                _ = ctorBody.AppendLine($"this.{fieldName} = {Argument.Name};");
            }
        }
        ctor.Body = ctorBody.ToString();
        _ = controllerClass.AddMember(ctor);

        // Add APIs
        foreach (var api in viewModel.Apis)
        {
            var method = new Method(api.Name!)
            {
                Body = api.Body,
                ReturnType = api.ReturnType,
            };
            _ = controllerClass.AddMember(api.Routes.Select(route => method.AddAttribute(TypePath.New<RouteAttribute>(), (null, route))));
            //foreach (var route in api.Routes)
            //{
            //    _ = method.AddAttribute(TypePath.New<RouteAttribute>(), (null, route));
            //}
            //_ = controllerClass.AddMember(method);
        }

        _ = ns.AddType(controllerClass);

        // Generate code
        var codeStatement = codeGeneratorEngine.Generate(ns);
        if (codeStatement.IsFailure)
        {
            return codeStatement.WithValue(Codes.Empty)!;
        }
        var partCode = Code.New(viewModel.ControllerName, Languages.CSharp, codeStatement, true);
        var mainCode = Code.Empty;

        // Return result
        return Codes.New(mainCode, partCode);
    }
}