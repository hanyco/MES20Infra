using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.Results;

namespace Services;

internal sealed class ApiCodingService(ICodeGeneratorEngine codeGeneratorEngine) : IApiCodingService
{
    public Result<Codes?> GenerateCodes(ApiCodingViewModel viewModel, ApiCodingArgs? arguments = null)
    {
        var code = Code.Empty;
        return code.ToCodes();
    }
}