using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IControllerService : IBusinessService, ICodeGenerator<ControllerViewModel>, IAsyncCrud<ControllerViewModel>;