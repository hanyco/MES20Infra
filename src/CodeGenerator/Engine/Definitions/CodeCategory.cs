using Library.CodeGeneration.Models;

namespace HanyCo.Infra.CodeGeneration.Definitions;

public enum CodeCategory
{
    Dto,
    Query,
    Command,
    Page,
    Component,
    Converter,
}

[Immutable]
public record GenerateCodesArgs(in bool GenerateMainCode = true, in bool GeneratePartialCode = true, in bool GenerateUiCode = true, in string? FileName = null)
    : GenerateCodesParameters(GenerateMainCode, GeneratePartialCode, GenerateUiCode);