using System.Text;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;

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
        _ = viewModel.ArgumentNotNull();

        var vr = viewModel.Check()
            .NotNull(x => x.Name)
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

        // Add ctor to controller, if required
        if (viewModel.CtorParams.Count != 0)
        {
            var ctor = new Method(controllerClass.Name);
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
        }

        // Add APIs
        foreach (var api in viewModel.Apis)
        {
        }

        // Generate code
        var codeStatement = codeGeneratorEngine.Generate(ns);
        if (codeStatement.IsFailure)
        {
            return codeStatement.WithValue(Codes.Empty)!;
        }
        var partCode = Code.New(viewModel.Name!, Languages.CSharp, codeStatement, true);

        var mainCode = Code.Empty;
        return Codes.New(mainCode, partCode);
    }
}