using Contracts.Services;

using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Mapping;

using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.UI.Helpers;

internal static class EntityViewModelConverterHelper
{

    public static async Task<TViewModel?> ToViewModelAsync<TViewModel, TDbEntity>([DisallowNull] this IDbEntityToViewModelConverter<TViewModel, TDbEntity> converter, TDbEntity? entity, ISecurityDescriptorService securityDescriptorService)
        where TViewModel : IHasSecurityDescriptor
    {
        var viewModel = converter.ToViewModel(entity);
        if (viewModel is not null)
        {
            var secs = viewModel.Guid is { } guid ? await securityDescriptorService.GetByEntityIdAsync(guid) : Enumerable.Empty<SecurityDescriptorViewModel>();
            secs.ForEach(viewModel.SecurityDescriptors.Add);
        }
        return viewModel;
    }

    [return: NotNull]
    public static async IAsyncEnumerable<TViewModel?> ToViewModelAsync<TViewModel, TDbEntity>([DisallowNull] this IDbEntityToViewModelConverter<TViewModel, TDbEntity> converter, IEnumerable<TDbEntity?> entities, ISecurityDescriptorService securityDescriptorService)
        where TViewModel : IHasSecurityDescriptor
    {
        await foreach (var vewModel in entities.SelectAsync(entity => ToViewModelAsync(converter, entity, securityDescriptorService)))
        {
            yield return vewModel;
        }
    }
    public static TViewModel? ToViewModel<TViewModel, TDbEntity>([DisallowNull] this IDbEntityToViewModelConverter<TViewModel, TDbEntity> converter, TDbEntity? entity, IEnumerable<SecurityDescriptorViewModel>? securityDescriptors)
        where TViewModel : IHasSecurityDescriptor =>
        MapperExtensions.ForMemberIfNotNull(converter.ToViewModel(entity), x => securityDescriptors?.ForEach(x.SecurityDescriptors.Add));

    [return: NotNull]
    public static IEnumerable<TViewModel?> ToViewModel<TViewModel, TDbEntity>([DisallowNull] this IDbEntityToViewModelConverter<TViewModel, TDbEntity> converter, IEnumerable<(TDbEntity? Entity, IEnumerable<SecurityDescriptorViewModel> SecurityDescriptors)> entitiesInfo)
        where TViewModel : IHasSecurityDescriptor
        => entitiesInfo.Select(x => ToViewModel(converter, x.Entity, x.SecurityDescriptors));
}
