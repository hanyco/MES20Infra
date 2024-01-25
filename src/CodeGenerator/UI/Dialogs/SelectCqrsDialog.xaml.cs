using System.Windows;




using Library.Exceptions.Validations;
using Library.Globalization;
using Library.Results;
using Library.Validations;

namespace HanyCo.Infra.UI.Dialogs;

/// <summary>
/// Interaction logic for SelectCqrsDialog.xaml
/// </summary>
public partial class SelectCqrsDialog
{
    private readonly ShowArgs _args;

    public SelectCqrsDialog(ShowArgs args)
    {
        this.Owner = Application.Current.MainWindow;
        this._args = args.ArgumentNotNull();
        this.Title = this._args.Title;
        this.InitializeComponent();
    }

    [Flags]
    public enum FilterDto
    {
        Params = 2,
        Result = 4,
        ViewModel = 8,
        All = Params | Result | ViewModel
    }

    [Flags]
    public enum LoadEntity
    {
        Dto = 2,
        Queries = 4,
        Commands = 8,
        All = Dto | Queries | Commands
    }

    public InfraViewModelBase? SelectedItem { get; set; }

    public static bool Show<TViewModel>(out TViewModel? result, in ShowArgs args)
        where TViewModel : InfraViewModelBase
    {
        var showResult = Show<TViewModel>(args);
        result = showResult;
        return showResult.IsSucceed;
    }

    public static TryMethodResult<TViewModel?> Show<TViewModel>(in ShowArgs args)
        where TViewModel : InfraViewModelBase
    {
        Check.MustBeArgumentNotNull(args);

        if (!args.Entities.Contains(LoadEntity.Dto) && !args.Entities.Contains(LoadEntity.Queries) && !args.Entities.Contains(LoadEntity.Commands))
        {
            return result(false, null, "Invalid argument value.");
        }

        var dlg = new SelectCqrsDialog(args);
        args.OnInitialize?.Invoke(dlg);
        dlg.SetupExplorer();
        return dlg.ShowDialog() is not true
            ? result(false, null)
            : (args.Entities == LoadEntity.Dto, args.Entities == LoadEntity.Queries, args.Entities == LoadEntity.Commands, dlg.SelectedItem) switch
            {
                (true, _, _, DtoViewModel item) => result(true, item.Cast().As<TViewModel>()),
                (true, _, _, _) => result(true, null),

                (_, true, _, CqrsViewModelBase item) => result(true, item.Cast().As<TViewModel>()),
                (_, true, _, _) => result(true, null),

                (_, _, true, CqrsViewModelBase item) => result(true, item.Cast().As<TViewModel>()),
                (_, _, true, _) => result(true, null),

                _ => throw new global::System.NotImplementedException()
            };
        static TryMethodResult<TViewModel?> result(bool result, TViewModel? model, string? message = null)
            => TryMethodResult<TViewModel?>.TryParseResult(result, model, message: message);
    }

    private async void OkButton_Click(object sender, RoutedEventArgs e)
    {
        ValidationException.ThrowIfNotValid(this.Explorer.SelectedItem is not CqrsViewModelBase and not DtoViewModel, "Please select an item");
        InfraViewModelBase? obj = this.Explorer.SelectedItem switch
        {
            DtoViewModel dto => await this.Service<IDtoService>().GetByIdAsync(dto.Id!.Value),
            CqrsViewModelBase cqrs => cqrs,
            _ => throw new ValidationException("Please select an item")
        };
        this.SelectedItem = obj;
        this.DialogResult = true;
    }

    private void SetupExplorer()
    {
        this.Explorer.LoadDtos = this._args.Entities.Contains(LoadEntity.Dto);
        this.Explorer.LoadQueries = this._args.Entities.Contains(LoadEntity.Queries);
        this.Explorer.LoadCommands = this._args.Entities.Contains(LoadEntity.Commands);
        this.Explorer.FilterDtoParams = this._args.FilterDto.Contains(FilterDto.Params);
        this.Explorer.FilterDtoResult = this._args.FilterDto.Contains(FilterDto.Params);
        this.Explorer.FilterViewModel = this._args.FilterDto.Contains(FilterDto.ViewModel);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
    }

    public sealed record ShowArgs(string Title, LoadEntity Entities, FilterDto FilterDto = FilterDto.All, Action<SelectCqrsDialog>? OnInitialize = null);
}