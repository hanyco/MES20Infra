


using HanyCo.Infra.UI.Services;


namespace HanyCo.Infra.UI.Helpers;

internal static class CqrsSegregationHelper
{
    public static async Task<CqrsViewModelBase?> FillAsync(this CqrsViewModelBase? model,
                                                           ICqrsQueryService? queryService = null,
                                                           ICqrsCommandService? commandService = null)
        => model switch
        {
            CqrsCommandViewModel cmd => await (commandService ?? DI.GetService<ICqrsCommandService>())!.FillViewModelAsync(cmd),
            CqrsQueryViewModel query => await (queryService ?? DI.GetService<ICqrsQueryService>())!.FillViewModelAsync(query),
            _ => null
        };
}
