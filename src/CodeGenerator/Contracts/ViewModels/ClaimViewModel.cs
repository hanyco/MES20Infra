using System.Diagnostics;

namespace Contracts.ViewModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ClaimViewModel : InfraViewModelBase<Guid>, IEquatable<ClaimViewModel>
{
    public string? Key { get; set; }

    public ClaimViewModel? Parent { get; set; }

    public bool Equals(ClaimViewModel? other) =>
        other?.Key == this.Key;

    public override bool Equals(object? obj) =>
        this.Equals(obj as ClaimViewModel);

    public override int GetHashCode() =>
        this.Key?.GetHashCode() ?? 0;

    private string GetDebuggerDisplay() =>
        this.ToString();
    public override string ToString() =>
        $"{this.Guid} - {this.Key}";
}