namespace Contracts.ViewModels;

public sealed class UiComponentCqrsLoadViewModel : InfraViewModelBase, IUiComponentCqrsContent, BackElement
{
    private CqrsViewModelBase? _cqrsSegregate;

    public UiComponentCqrsLoadViewModel()
        : base(null, name: "OnCqrsLoad")
    {
    }

    public CqrsViewModelBase? CqrsSegregate { get => this._cqrsSegregate; set => this.SetProperty(ref this._cqrsSegregate, value); }
}
