using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Results;
using Library.Validations;

namespace Services;

internal sealed class ApiCodingService(ICodeGeneratorEngine codeGeneratorEngine) : IApiCodingService
{
    public Result<Codes> GenerateCodes(ApiCodingViewModel viewModel, ApiCodingArgs? arguments = null)
    {
        if (!Check.IfArgumentIsNull(viewModel).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }

        var controller = new Class($"{viewModel.TypeName}Controller");
        controller.AddAttribute("Microsoft.AspNetCore.Mvc.ApiControllerAttribute");
        controller.AddAttribute("Microsoft.AspNetCore.Mvc.RouteAttribute", (null, "[controller]"));
        var ns = INamespace.New(viewModel.NameSpace).AddType(controller);

        var statement = codeGeneratorEngine.Generate(ns);
        var code = new Code(type.Name, Languages.CSharp, statement.Value)
            .With(x => x.props().Category = CodeCategory.Api);
        return Codes.Empty;
    }
}