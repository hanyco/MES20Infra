namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public sealed class UiComponentCqrsButtonViewModel : UiComponentButtonViewModelBase, IUiComponentCqrsContent, FrontElement
{
    private CqrsViewModelBase? _cqrsSegregate;

    public CqrsViewModelBase? CqrsSegregate { get => this._cqrsSegregate; set => this.SetProperty(ref this._cqrsSegregate, value); }
}
