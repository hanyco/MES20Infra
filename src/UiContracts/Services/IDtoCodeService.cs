using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface IDtoCodeService : IBusinesService, ICodeGeneratorService<DtoViewModel>
{

}
