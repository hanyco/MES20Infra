using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Results;
using Library.Validations;

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

        var ns = INamespace.New(viewModel.ControllerName);

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