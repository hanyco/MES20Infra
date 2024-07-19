using Library.Interfaces;
using Library.Types;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class ClaimViewModel : InfraViewModelBase<Guid>, IEmpty<ClaimViewModel>
{
    public const string DEFAULT_CLAIM_TYPE = "Default";
    public const string DEFAULT_CLAIM_VALUE = "Default";

    private static ClaimViewModel? _empty;
    private string? _claimType;
    private string? _claimValue;

    public static ClaimViewModel Empty => _empty ??= NewEmpty();

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

    public Guid? SecuritytDescriptorId { get; set; }

    public static ClaimViewModel Edit(Id id, Id parentId, string type, string value, string? description = null)
        => new() { Id = id, SecuritytDescriptorId = id, ClaimType = type, ClaimValue = value, Description = description };

    public static ClaimViewModel New(string type, string value, string? description = null, Id? parentId = null)
        => new() { ClaimType = type, ClaimValue = value, Description = description, SecuritytDescriptorId = parentId };

    public static ClaimViewModel NewDefault(Id? parentId = null)
        => new() { ClaimType = DEFAULT_CLAIM_TYPE, ClaimValue = DEFAULT_CLAIM_VALUE, SecuritytDescriptorId = parentId };

    public static ClaimViewModel NewEmpty()
        => new();
}