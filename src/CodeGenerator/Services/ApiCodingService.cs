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
            var ctorBody = new StringBuilder();
            var ctorParams = new List<MethodArgument>();

            foreach (var ctorParam in viewModel.CtorParams)
            {
                if (ctorParam.IsField)
                {
                    var fieldName = TypeMemberNameHelper.ToFieldName(ctorParam.Argument.Name);
                    controllerClass.AddField(fieldName, ctorParam.Argument.Type);
                    ctorBody.AppendLine($"this.{fieldName} = {ctorParam.Argument.Name};");
                }
                ctorParams.Add(ctorParam.Argument);
            }
            var cror = IMethod.New(controllerClass.Name).Body
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