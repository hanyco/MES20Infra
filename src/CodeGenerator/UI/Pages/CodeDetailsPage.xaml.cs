using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Validations;
using Library.Wpf.Dialogs;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for CodeDetailsPage.xaml
/// </summary>
public partial class CodeDetailsPage
{
    private readonly ICqrsCodeGeneratorService _CodeGeneratorService;
    private readonly ICqrsCommandService _CommandService;
    private readonly IDtoService _DtoService;
    private readonly ICqrsQueryService _QueryService;

    #region bool AutoPreview

    public static readonly DependencyProperty AutoPreviewProperty = ControlHelper.GetDependencyProperty<bool, CodeDetailsPage>(nameof(AutoPreview));

    public bool AutoPreview
    {
        get => (bool)this.GetValue(AutoPreviewProperty);
        set => this.SetValue(AutoPreviewProperty, value);
    }

    #endregion bool AutoPreview

    #region string? CqrsSegregateTitle

    public static readonly DependencyProperty CqrsSegregateTitleProperty = ControlHelper.GetDependencyProperty<string?, CodeDetailsPage>(nameof(CqrsSegregateTitle));

    public string? CqrsSegregateTitle
    {
        get => (string?)this.GetValue(CqrsSegregateTitleProperty);
        set => this.SetValue(CqrsSegregateTitleProperty, value);
    }

    #endregion string? CqrsSegregateTitle

    #region Codes? Codes

    public static readonly DependencyProperty CodesProperty = ControlHelper.GetDependencyProperty<Codes, CodeDetailsPage>(nameof(Codes));

    public Codes? Codes
    {
        get => (Codes)this.GetValue(CodesProperty);
        set => this.SetValue(CodesProperty, value);
    }

    #endregion Codes? Codes

    #region Code? SelectedCode

    public static readonly DependencyProperty SelectedCodeProperty = ControlHelper.GetDependencyProperty<string?, CodeDetailsPage>(nameof(SelectedCode));

    public string? SelectedCode
    {
        get => (string?)this.GetValue(SelectedCodeProperty);
        set => this.SetValue(SelectedCodeProperty, value);
    }

    #endregion Code? SelectedCode

    public CodeDetailsPage(IDtoService dtoService,
                           ICqrsQueryService queryService,
                           ICqrsCommandService commandService,
                           ICqrsCodeGeneratorService codeGeneratorService, ILogger logger)
        : base(logger)
    {
        this.SelectedCode = Code._empty;
        this._DtoService = dtoService;
        this._QueryService = queryService;
        this._CommandService = commandService;
        this._CodeGeneratorService = codeGeneratorService;
        this.InitializeComponent();
    }

    private async void CodeDetailsPage_Binding(object sender, EventArgs e)
    {
        var queriesTreeViewItem = GetTreeItem(await this._QueryService.GetAllAsync());
        _ = this.CqrsTreeView.Items.Add(await this._QueryService.GetAllAsync());

        var commandsTreeViewItem = GetCommandItems(await this._CommandService.GetAllAsync());
        _ = this.CqrsTreeView.Items.Add(commandsTreeViewItem);

        static TreeViewItem GetTreeItem(IEnumerable<CqrsQueryViewModel> segregates)
        {
            var result = new TreeViewItem { Header = "Queries" };
            _ = segregates.CreateIterator(x => result.Items.Add(new TreeViewItem { DataContext = x, Header = x.ToString() })).Build();
            return result;
        }

        static TreeViewItem GetCommandItems(IEnumerable<CqrsCommandViewModel> segregates)
        {
            var resiult = new TreeViewItem { Header = "Commands" };
            _ = segregates.CreateIterator(x => resiult.Items.Add(new TreeViewItem { DataContext = x, Header = x.ToString() })).Build();
            return resiult;
        }
    }

    private void CodesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (this.Codes is null)
        {
            return;
        }
        var code = e.GetSelection<Code>();
        this.SelectedCode = code;
        this.IsPartialCheckBox.IsChecked = code.IsPartial;
    }

    private async void CqrsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var selectedValue = e.NewValue.Cast().As<TreeViewItem>()?.DataContext;
        if (selectedValue is not null and CqrsViewModelBase viewModel)
        {
            this.GenerateCommandButton.IsEnabled = true;
            if (this.AutoPreview)
            {
                await this.FillCodeAsync(viewModel);
            }
        }
        else
        {
            this.GenerateCommandButton.IsEnabled = false;
        }
    }

    private async Task FillCodeAsync(CqrsViewModelBase viewModel)
    {
        this.Codes = await this._CodeGeneratorService.GenerateCodeAsync(viewModel);
        this.CqrsSegregateTitle = viewModel.Name;
    }

    private async void GenerateCommandButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedValue = this.CqrsTreeView.SelectedValue.Cast().As<TreeViewItem>()?.DataContext;
        if (selectedValue is not null and CqrsViewModelBase viewModel)
        {
            await this.FillCodeAsync(viewModel);
        }
    }

    private async void SaveAllToFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.Codes?.Any() is not true)
        {
            return;
        }

        using var dlg = new FolderBrowserDialog();
        if (dlg.ShowDialog() is not DialogResult.OK)
        {
            return;
        }
        var folder = dlg.SelectedPath;
        foreach (var code in this.Codes.Compact())
        {
            var asPartial = code.IsPartial ? ".partial" : "";
            var filePath = Path.Combine(folder, $"{code.Name}{asPartial}.cs");
            await File.WriteAllTextAsync(filePath, code.Statement);
        }
        MsgBox2.Inform("Code statements saved.");
    }

    private async void SaveToFileButton_Click(object sender, RoutedEventArgs e)
    {
        Code code = this.CodesComboBox.SelectedItem.Cast().As<Code>().NotNull(() => "No code found.");
        
        var asPartial = code.IsPartial ? ".partial" : "";
        var filePath = $"{code.Name}{asPartial}.cs";
        var (isOk, fileName) = CommonDialogHelper.Save(filePath, "cs", "C# Code (*.cs)|*.cs");
        if (!isOk)
        {
            return;
        }

        await File.WriteAllTextAsync(fileName, code.Statement);
        MsgBox2.Inform("Code statement saved.");
    }
}