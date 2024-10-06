using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IModuleService : IBusinessService, IAsyncRead<ModuleViewModel>;