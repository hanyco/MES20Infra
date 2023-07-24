using Contracts.ViewModels;

using Library.Interfaces;
using Library.Types;

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class SecurityDescriptorViewModel : InfraViewModelBase<Id>, IEmpty<SecurityDescriptorViewModel>
{
    private bool _isEnabled = true;
    private static SecurityDescriptorViewModel? _empty;
    private bool _isClaimBased;
    private bool _isNoSec = true;

    public static SecurityDescriptorViewModel Empty => _empty ??= NewEmpty();

    [Required]
    public Guid EntityId { get; set; } //! To avoid the need for additional intermediate view models

    public ObservableCollection<ClaimViewModel> ClaimSet { get; } = new();

    public bool IsEnabled
    {
        get => this._isEnabled;
        set
        {
            if (this._isEnabled != value)
            {
                this.SetProperty(ref this._isEnabled, value);
            }
        }
    }
    public bool IsNoSec
    {
        get => this._isNoSec;
        set
        {
            if (this._isNoSec == value)
            {
                return;
            }

            this.IsClaimBased = false;
            this.SetProperty(ref this._isNoSec, value);
        }
    }
    public bool IsClaimBased
    {
        get => this._isClaimBased;
        set
        {
            if (this._isClaimBased == value)
            {
                return;
            }

            this.IsNoSec = false;
            this.SetProperty(ref this._isClaimBased, value);
        }
    }

    public static SecurityDescriptorViewModel NewEmpty() =>
        new();

    [Obsolete("کاربردی ندارد.", true)]
    public new Guid Guid { get; }

    public static SecurityDescriptorViewModel New() =>
        new();

    public override string ToString() =>
        this.Name ?? "(No Name)";
}