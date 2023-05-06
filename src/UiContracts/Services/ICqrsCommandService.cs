using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface ICqrsCommandService : IBusinessService, IAsyncSaveService, IAsyncCrudService<CqrsCommandViewModel>, IAsyncCreator<CqrsCommandViewModel>
{
    Task<CqrsCommandViewModel> FillByDbEntity(CqrsCommandViewModel model,
        long id,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null);

    //CqrsCommandViewModel FillByDbEntity(CqrsCommandViewModel model,
    //    CqrsSegregate sergregate,
    //    Module mesModule,
    //    Dto parameterDto,
    //    IEnumerable<Property> parameterDtoProperties,
    //    Dto resultDto,
    //    IEnumerable<Property> resultDtoProperties);

    Task<CqrsCommandViewModel> FillViewModelAsync(CqrsCommandViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null);

    //Task<IReadOnlyList<CqrsCommandViewModel>> GetCommandsByDtoIdAsync(long dtoId);
}