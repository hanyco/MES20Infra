﻿using System.Diagnostics.CodeAnalysis;
using System.Windows;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.UserControls;
using HanyCo.Infra.UI.ViewModels;

using Library.EventsArgs;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;

namespace HanyCo.Infra.UI.Pages;

/// <summary>
/// Interaction logic for FunctionalityEditorPage.xaml
/// </summary>
public partial class FunctionalityEditorPage : IStatefulPage, IAsyncSavePage
{
    private readonly IModuleService _moduleService;
    private readonly IFunctionalityService _service;
    private CqrsExplorerTreeView? _dtoExplorerTreeView;

    public FunctionalityEditorPage(
        IFunctionalityService service,
        IFunctionalityCodeService codeService,
        IModuleService moduleService,
        IDbTableService dbTableService,
        IMultistepProcess reporter,
        ILogger logger)
        : base(logger)
    {
        this.InitializeComponent();
        this._service = service;
        this._moduleService = moduleService;
    }

    bool IStatefulPage.IsViewModelChanged { get; set; }

    public FunctionalityViewModel? ViewModel
    {
        get => this.GetViewModelByDataContext<FunctionalityViewModel>();
        set => this.SetViewModelByDataContext(value);
    }

    public async Task<Result<int>> SaveAsync()
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync();

        _ = await this._service.SaveViewModelAsync(this.ViewModel!).ThrowOnFailAsync();
        _ = this.SetIsViewModelChanged(false);
        return Result<int>.CreateSuccess(0);
    }

    protected override Task<Result> OnValidateFormAsync()
    {
        this.CheckIfInitiated();
        return base.OnValidateFormAsync();
    }

    [MemberNotNull(nameof(ViewModel))]
    private void CheckIfInitiated()
        => this.ViewModel.NotNull(() => "Please create a new Functionality or edit an old one.");

    private async void CreateFunctionalityButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.AskToSaveAsync().BreakOnFail();
        this.ViewModel = new();
    }

    private async void GenerateCodesButton_Click(object sender, RoutedEventArgs e)
    {
        _ = await this.ValidateFormAsync().ThrowOnFailAsync();

        var scope = this.BeginActionScope("Generating...");
        this.ViewModel = await this._service.GenerateViewModelAsync(this.ViewModel!)
                                            .WithAsync(x => scope.End(x))
                                            .ThrowOnFailAsync(this.Title);
    }

    private void Me_Loaded(object sender, RoutedEventArgs e) => 
        this.ViewModel = new();

    private void ModuleComboBox_Initializing(object sender, InitialItemEventArgs<IModuleService> e)
            => e.Item = this._moduleService;

    private void ModuleComboBox_SelectedModuleChanged(object sender, ItemActedEventArgs<ModuleViewModel> e)
    {
        this.CheckIfInitiated();

        if (e.Item?.Id is { } moduleId)
        {
            this.ViewModel.ModuleId = moduleId;
        }
    }

    private void SelectRootDtoButton_Click(object sender, RoutedEventArgs e)
    {
        this.CheckIfInitiated();

        this._dtoExplorerTreeView ??= new CqrsExplorerTreeView() { LoadDtos = true };
        var isSelected = HostDialog.ShowDialog(this._dtoExplorerTreeView, "Select Root DTO", "Select a DTO to create a Functionality.", _ => Check.MustBe(this._dtoExplorerTreeView.SelectedItem is DtoViewModel, () => "Please select a DTO."));
        if (isSelected && this._dtoExplorerTreeView.SelectedItem is DtoViewModel { } dto)
        {
            this.ViewModel.DbObjectViewModel = dto.DbObject;
        }
    }
}