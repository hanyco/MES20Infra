﻿using System.Text;

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
    public Result<Codes> GenerateCodes(ApiCodingViewModel viewModel)
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
            .AddAttribute(TypePath.New<ApiControllerAttribute>())
            .AddAttribute<RouteAttribute>((null, "\"[controller]\""));
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

        foreach (var (argument, isField) in viewModel.CtorParams)
        {
            _ = ctor.Arguments.Add(argument);
            if (isField)
            {
                var fieldName = TypeMemberNameHelper.ToFieldName(argument.Name);
                _ = ns.AddUsingNameSpace(argument.Type.GetNameSpaces());
                _ = controllerClass.AddField(fieldName, argument.Type);
                _ = ctorBody.AppendLine($"this.{fieldName} = {argument.Name};");
            }
        }
        ctor.Body = ctorBody.ToString();
        _ = controllerClass.AddMember(ctor);

        // Add APIs
        foreach (var api in viewModel.Apis)
        {
            // Parse return type
            string returnType;
            if (api.ReturnType is not null)
            {
                returnType = api.ReturnType.FullName;
                _ = ns.AddUsingNameSpace(api.ReturnType.GetNameSpaces());
            }
            else
            {
                returnType = "void";
            }

            // Create method
            var method = new Method(api.Name!)
            {
                Body = api.Body,
                ReturnType = returnType
            };

            // Add HTTP Methods
            foreach (var httpMethod in api.HttpMethods)
            {
                var httpType = httpMethod.GetType();
                var route = httpType.GetProperty(nameof(httpMethod.Template))!.GetValue(httpMethod)?.ToString();
                _ = route.IsNullOrEmpty()
                    ? method.AddAttribute(TypePath.New(httpType))
                    : method.AddAttribute(TypePath.New(httpType), ((string? Name, string Value))(null, route));
                _ = ns.AddUsingNameSpace(httpType.Namespace!);
            }

            // Add API to Controller
            _ = controllerClass.AddMember(method);
        }

        // Add Controller to Namespace
        _ = ns.AddType(controllerClass);

        // Add additional usings to namespace
        foreach (var nameSpace in viewModel.AdditionalUsings)
        {
            _ = ns.AddUsingNameSpace(nameSpace);
        }

        // Generate code
        var codeStatement = codeGeneratorEngine.Generate(ns);
        if (codeStatement.IsFailure)
        {
            return codeStatement.WithValue(Codes.Empty)!;
        }
        var partCode = Code.New(viewModel.ControllerName, Languages.CSharp, codeStatement, true);

        // TODO: Add main part to let the developer to add his/her own code to Controller
        var mainCode = Code.Empty;

        // Return result
        return Codes.New(mainCode, partCode);
    }
}