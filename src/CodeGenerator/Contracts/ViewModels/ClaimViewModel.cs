namespace Contracts.ViewModels;

public sealed class ClaimViewModel : InfraViewModelBase<Guid>
{
    public string? Key { get; set; }

    public ClaimViewModel? Parent { get; set; }
}