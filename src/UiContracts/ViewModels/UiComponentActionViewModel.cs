using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

namespace HanyCo.Infra.UI.ViewModels
{
    public sealed class UiComponentActionViewModel : UiComponentContentViewModelBase
    {
        #region Fields

        private CqrsViewModelBase? _CqrsSegregate;
        private string? _EventHandlerName;
        private TriggerType _TriggerType;

        #endregion

        public CqrsViewModelBase? CqrsSegregate { get => this._CqrsSegregate; set => this.SetProperty(ref this._CqrsSegregate, value); }

        public string? EventHandlerName { get => this._EventHandlerName; set => this.SetProperty(ref this._EventHandlerName, value); }

        public TriggerType TriggerType { get => this._TriggerType; set => this.SetProperty(ref this._TriggerType, value); }
    }
}
