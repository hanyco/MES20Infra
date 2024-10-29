using Library.Results;

namespace HanyCo.Infra.UI.Pages;

public class PageBase : Library.Wpf.Bases.LibPageBase, ILoggerContainer
{
    [Obsolete("This .ctor is reserved for designer.")]
    public PageBase()
    { }

    public PageBase(ILogger logger)
    {
        this.Logger = logger;
        this.Loaded += this.PageBase_Loaded;
    }

    ILogger ILoggerContainer.Logger => this.Logger;

    protected ILogger Logger { get; }

    protected virtual async Task<Result> OnValidateFormAsync(CancellationToken cancellationToken = default)
        => await Task.FromResult(Result.Succeed);

    protected async Task<Result> ValidateFormAsync(CancellationToken cancellationToken = default)
        => await this.OnValidateFormAsync(cancellationToken);

    private void PageBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        => this.EndActionScope();
}