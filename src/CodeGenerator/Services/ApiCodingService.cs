using System.Text;

using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services;

internal sealed class ApiCodingService(ICodeGeneratorEngine codeGeneratorEngine) : IApiCodingService
{
    public Result<Codes?> GenerateCodes(ApiCodingViewModel viewModel, ApiCodingArgs? arguments = null)
    {
        viewModel.ArgumentNotNull();

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
            controllerClass.AddAttribute(typeof(AllowAnonymousAttribute));
        }

        // Add ctor to controller, if required
        if (viewModel.CtorParams.Count != 0)
        {
            var ctor = new Method(controllerClass.Name);
            var ctorBody = new StringBuilder();

            foreach (var ctorParam in viewModel.CtorParams)
            {
                ctor.Arguments.Add(ctorParam.Argument);
                if (ctorParam.IsField)
                {
                    var fieldName = TypeMemberNameHelper.ToFieldName(ctorParam.Argument.Name);
                    controllerClass.AddField(fieldName, ctorParam.Argument.Type);
                    ctorBody.AppendLine($"this.{fieldName} = {ctorParam.Argument.Name};");
                }
            }
            ctor.Body = ctorBody.ToString();
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