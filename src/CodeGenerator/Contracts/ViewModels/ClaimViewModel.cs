namespace Contracts.ViewModels;

public sealed class ClaimViewModel : InfraViewModelBase<Guid>
{
    private string _claimType = string.Empty;
    private string _claimValue = string.Empty;

    public string ClaimType
    {
        get => this._claimType ?? string.Empty;
        set => this.SetProperty(ref this._claimType, value);
    }

    public string ClaimValue
    {
        get => this._claimValue ?? string.Empty;
        set => this.SetProperty(ref this._claimValue, value);
    }
}