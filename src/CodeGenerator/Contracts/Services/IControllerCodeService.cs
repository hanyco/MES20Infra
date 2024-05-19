using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.DesignPatterns.Markers;
using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;
public interface IControllerCodeService : IBusinessService
    , ICodeGenerator<ControllerViewModel, ControllerCodeServiceGenerateCodesParameters>
{
}

[Immutable]
public sealed record ControllerCodeServiceGenerateCodesParameters(
    bool generateGetAll = true,
    bool generateGetById = true,
    bool generateInsert = true,
    bool generateUpdate = true,
    bool generateDelete = true);
