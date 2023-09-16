using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace Contracts.Services;

public interface IConverterCodeGeneratorService : IBusinessService, ICodeGenerator<ConverterCodeGeneratorArgs>
{
}

public readonly struct ConverterCodeGeneratorArgs
{
}