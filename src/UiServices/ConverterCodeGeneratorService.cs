using Contracts.Services;

using Library.CodeGeneration.Models;
using Library.Results;

namespace Services;

internal sealed class ConverterCodeGeneratorService : IConverterCodeGeneratorService
{
    public Result<Codes> GenerateCodes(ConverterCodeGeneratorArgs viewModel, GenerateCodesParameters? arguments = null) 
        => throw new NotImplementedException();
}