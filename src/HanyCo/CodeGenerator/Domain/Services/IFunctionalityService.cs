using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IFunctionalityService : IBusinessService, IAsyncCrud<FunctionalityViewModel>, IAsyncCreator<FunctionalityViewModel>
{
}