using System.IO;
using System.Windows;
using System.Windows.Forms;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Collections;
using Library.Exceptions.Validations;
using Library.Validations;
using Library.Wpf.Dialogs;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for CreateTableCrudPage.xaml
/// </summary>
public partial class CreateTableCrudPage
{
    private readonly ICqrsCodeGeneratorService _CodeGeneratorService;
    private readonly IDbTableService _DbTableService;
    private readonly IModuleService _ModuleService;

    #region Dependency Properties

    #region Modules

    public static readonly DependencyProperty ModulesProperty = DependencyProperty.Register(
        nameof(Modules),
        typeof(IEnumerable<ModuleViewModel>),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(null));

    public IEnumerable<ModuleViewModel> Modules
    {
        get => (IEnumerable<ModuleViewModel>)this.GetValue(ModulesProperty);
        set => this.SetValue(ModulesProperty, value);
    }

    #endregion Modules

    #region SelectedModule

    public static readonly DependencyProperty SelectedModuleProperty = DependencyProperty.Register(
        nameof(SelectedModule),
        typeof(ModuleViewModel),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(null));

    public ModuleViewModel? SelectedModule
    {
        get => (ModuleViewModel)this.GetValue(SelectedModuleProperty);
        set => this.SetValue(SelectedModuleProperty, value);
    }

    #endregion SelectedModule

    #region SelectedTable

    public static readonly DependencyProperty SelectedTableProperty = DependencyProperty.Register(
        nameof(SelectedTable),
        typeof(Node<DbObjectViewModel>),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(null));

    public Node<DbObjectViewModel>? SelectedTable
    {
        get => (Node<DbObjectViewModel>)this.GetValue(SelectedTableProperty);
        set => this.SetValue(SelectedTableProperty, value);
    }

    #endregion SelectedTable

    #region ShouldGenerateGetAll

    public static readonly DependencyProperty ShouldGenerateGetAllProperty = DependencyProperty.Register(
        nameof(ShouldGenerateGetAll),
        typeof(bool),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(true));

    public bool ShouldGenerateGetAll
    {
        get => (bool)this.GetValue(ShouldGenerateGetAllProperty);
        set => this.SetValue(ShouldGenerateGetAllProperty, value);
    }

    #endregion ShouldGenerateGetAll

    #region ShouldGenerateGetById

    public static readonly DependencyProperty ShouldGenerateGetByIdProperty = DependencyProperty.Register(
        nameof(ShouldGenerateGetById),
        typeof(bool),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(true));

    public bool ShouldGenerateGetById
    {
        get => (bool)this.GetValue(ShouldGenerateGetByIdProperty);
        set => this.SetValue(ShouldGenerateGetByIdProperty, value);
    }

    #endregion ShouldGenerateGetById

    #region ShouldGenerateCreate

    public static readonly DependencyProperty ShouldGenerateCreateProperty = DependencyProperty.Register(
        nameof(ShouldGenerateCreate),
        typeof(bool),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(true));

    public bool ShouldGenerateCreate
    {
        get => (bool)this.GetValue(ShouldGenerateCreateProperty);
        set => this.SetValue(ShouldGenerateCreateProperty, value);
    }

    #endregion ShouldGenerateCreate

    #region ShouldGenerateUpdate

    public static readonly DependencyProperty ShouldGenerateUpdateProperty = DependencyProperty.Register(
        nameof(ShouldGenerateUpdate),
        typeof(bool),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(true));

    public bool ShouldGenerateUpdate
    {
        get => (bool)this.GetValue(ShouldGenerateUpdateProperty);
        set => this.SetValue(ShouldGenerateUpdateProperty, value);
    }

    #endregion ShouldGenerateUpdate

    #region ShouldGenerateDelete

    public static readonly DependencyProperty ShouldGenerateDeleteProperty = DependencyProperty.Register(
        nameof(ShouldGenerateDelete),
        typeof(bool),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(true));

    public bool ShouldGenerateDelete
    {
        get => (bool)this.GetValue(ShouldGenerateDeleteProperty);
        set => this.SetValue(ShouldGenerateDeleteProperty, value);
    }

    #endregion ShouldGenerateDelete

    #region ShouldGeneratePartialOnInitialize

    public static readonly DependencyProperty ShouldGeneratePartialOnInitializeProperty = DependencyProperty.Register(
        nameof(ShouldGeneratePartialOnInitialize),
        typeof(bool),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(true));

    public bool ShouldGeneratePartialOnInitialize
    {
        get => (bool)this.GetValue(ShouldGeneratePartialOnInitializeProperty);
        set => this.SetValue(ShouldGeneratePartialOnInitializeProperty, value);
    }

    #endregion ShouldGeneratePartialOnInitialize

    #region ShouldGenerateDefaultMethodBodies

    public static readonly DependencyProperty ShouldGenerateDefaultMethodBodiesProperty = DependencyProperty.Register(
        nameof(ShouldGenerateDefaultMethodBodies),
        typeof(bool),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(true));

    public bool ShouldGenerateDefaultMethodBodies
    {
        get => (bool)this.GetValue(ShouldGenerateDefaultMethodBodiesProperty);
        set => this.SetValue(ShouldGenerateDefaultMethodBodiesProperty, value);
    }

    #endregion ShouldGenerateDefaultMethodBodies

    #region CqrsNamespace

    public static readonly DependencyProperty CqrsNamespaceProperty = DependencyProperty.Register(
        nameof(CqrsNamespace),
        typeof(string),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(null));

    public string? CqrsNamespace
    {
        get => (string)this.GetValue(CqrsNamespaceProperty);
        set => this.SetValue(CqrsNamespaceProperty, value);
    }

    #endregion CqrsNamespace

    #region DtoNamespace

    public static readonly DependencyProperty DtoNamespaceProperty = DependencyProperty.Register(
        nameof(DtoNamespace),
        typeof(string),
        typeof(CreateTableCrudPage),
        new PropertyMetadata(null));

    public string? DtoNamespace
    {
        get => (string)this.GetValue(DtoNamespaceProperty);
        set => this.SetValue(DtoNamespaceProperty, value);
    }

    #endregion DtoNamespace

    #endregion Dependency Properties

    public CreateTableCrudPage(
        IModuleService moduleService,
        IDbTableService dbTableService,
        ICqrsCodeGeneratorService codeGeneratorService, ILogger logger)
        : base(logger)
    {
        this._ModuleService = moduleService;
        this._DbTableService = dbTableService;
        this._CodeGeneratorService = codeGeneratorService;
        this.InitializeComponent();
    }

    private void CreateTableCrudPage_Binding(object sender, EventArgs e)
        => MsgBox2.ShowProgress(action: async (dlg, _, _) =>
                 {
                     dlg.InstructionText = "Gatheing modules...";
                     var dbResult = await this._ModuleService.GetAllAsync();
                     this.Modules = dbResult;
                     dlg.InstructionText = "Initializing...";
                     dlg.ProgressBar.Value = 30;
                     _ = await this.ServerExplorerTreeView.InitializeAsync(this._DbTableService);
                     dlg.Close();
                     this.Logger.Debug("Ready.");
                 }, showOkButton: false, isCancallable: false);

    private async void GenerateCodeButton_Click(object sender, RoutedEventArgs e)
    {
        this.Validate();

        if (!BooleanHelper.Any(
            this.ShouldGenerateGetAll,
            this.ShouldGenerateGetById,
            this.ShouldGenerateCreate,
            this.ShouldGenerateUpdate,
            this.ShouldGenerateDelete))
        {
            throw new ValidationException("No segregate is selected.", "Please selected select at least one of 'GetAll', 'Get By Id',….");
        }

        using var dlg = new FolderBrowserDialog();
        if (dlg.ShowDialog() != DialogResult.OK)
        {
            return;
        }
        var folder = dlg.SelectedPath;
        var entityName = this.SelectedTable.Value.Name;
        //var codes = this._CodeGeneratorService.GenerateAllCodes(
        //    new(entityName, this.SelectedTable, this.CqrsNamespace, this.DtoNamespace),
        //    new(this.ShouldGenerateGetAll,
        //        this.ShouldGenerateGetById,
        //        this.ShouldGenerateCreate,
        //        this.ShouldGenerateUpdate,
        //        this.ShouldGenerateDelete));

        //if (!codes.Any())
        //{
        //    throw new Exceptions.MesException("No CQRS Segregation is selected.");
        //}
        //foreach (var code in codes)
        //{
        //    foreach (var statement in code.Codes)
        //    {
        //        var suffix = statement.IsPartial ? ".Partial" : "";
        //        var sourceFile = Path.Combine(folder, $"{statement.Name}{suffix}.cs");
        //        using var stream = File.CreateText(sourceFile);
        //        await stream.WriteAsync(statement.Statement);
        //    }
        //}
        const string resultPrompt = "Code(s) saved.";
        this.Logger.Debug(resultPrompt);
        MsgBox2.Inform(resultPrompt);
    }

    private void NewCrudButton_Click(object sender, RoutedEventArgs e)
    {
        this.SelectedTable = null;
        this.Logger.Debug("Ready.");
    }

    private void SaveCrudButton_Click(object sender, RoutedEventArgs e)
    {
        this.Validate();
        var entityName = this.SelectedTable.Value.Name;
        //await this._CodeGeneratorService.SaveToDatabaseAsync(
        //    new(entityName, this.SelectedTable, this.CqrsNamespace, this.DtoNamespace),
        //    new(this.ShouldGenerateGetAll,
        //        this.ShouldGenerateGetById,
        //        this.ShouldGenerateCreate,
        //        this.ShouldGenerateUpdate,
        //        this.ShouldGenerateDelete));
        this.Logger.Debug("Meta data saved.");
    }

    private void SelectCrudButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedDbObjectNode = this.ServerExplorerTreeView.SelectedDbObjectNode.NotNull(() => new ValidationException("Please select a table."));

        if (selectedDbObjectNode!.Is<DbTableViewModel>())
        {
            this.SelectedTable = selectedDbObjectNode;
        }
        else
        {
            var parent = selectedDbObjectNode.Parent;
            while (parent is not null)
            {
                if (parent.Is<DbTableViewModel>())
                {
                    this.SelectedTable = parent!;
                    break;
                }
                parent = parent.Parent;
            }
        }
    }

    private void Validate()
    {
        _ = this.SelectedTable.NotNull(() => new ValidationException("No table is selected.", "Please select a table."));
        _ = this.SelectedTable.Value.NotNull(() => new ValidationException("No table is selected.", "Please select a table."));
        _ = this.CqrsNamespace.NotNull();
        _ = this.DtoNamespace.NotNull();
    }
}