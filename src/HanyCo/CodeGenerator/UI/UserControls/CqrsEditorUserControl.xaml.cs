using System.Windows.Controls;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.EventsArgs;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for CqrsEditorUserControl.xaml
/// </summary>
public partial class CqrsEditorUserControl : UserControl
{
    private IDtoService _dtoService = null!;
    private IModuleService _moduleService = null!;

    public CqrsEditorUserControl() => 
        this.InitializeComponent();

    public void Initialize(IModuleService moduleService, IDtoService dtoService)
    {
        (this._moduleService, this._dtoService) = (moduleService, dtoService);
        ControlHelper.BindItemsSourceToEnum<CqrsSegregateCategory>(CategoryComboBox, CqrsSegregateCategory.Read);
    }

    private async void SelectModuleBox_SelectedModuleChanged(object sender, ItemActedEventArgs<ModuleViewModel> e)
    {
        this.ResultDtoComboBox.ItemsSource = null;
        this.ParamDtoComboBox.ItemsSource = null;
        var moduleId = e.Item?.Id;
        if (moduleId is null)
        {
            return;
        }
        var dtos = await this._dtoService.GetByModuleId(moduleId.Value);

        var paramDtos = dtos.Where(x => x.IsParamsDto).OrderBy(x => x.Name);
        _ = this.ParamDtoComboBox.BindItemsSource(paramDtos, nameof(ModuleViewModel.Name));

        var resultDtos = dtos.Where(x => x.IsResultDto).OrderBy(x => x.Name);
        _ = this.ResultDtoComboBox.BindItemsSource(resultDtos, nameof(ModuleViewModel.Name));
    }

    private void SelectModuleUserControl_Initializing(object sender, InitialItemEventArgs<IModuleService> e)=> 
        e.Item = this._moduleService;
}