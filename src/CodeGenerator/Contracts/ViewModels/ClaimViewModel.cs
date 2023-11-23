using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Contracts.ViewModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ClaimViewModel : InfraViewModelBase<Guid>, IEquatable<ClaimViewModel>
{
    public ClaimViewModel()
        : this(null, null)
    {
    }

    public ClaimViewModel(string? key, Guid? guid = null) =>
        (this.Guid, this.Key) = (guid ?? System.Guid.NewGuid(), key);

    public ClaimViewModel(string? key, Guid? guid, ClaimViewModel? parent)
        : this(key, guid) => this.Parent = parent;

    public string? Key { get; set; }

    public ClaimViewModel? Parent { get; set; }

    public bool Equals(ClaimViewModel? other) =>
        other?.Key == this.Key;

    public override bool Equals(object? obj) =>
        this.Equals(obj as ClaimViewModel);

    public override int GetHashCode() =>
        this.Key?.GetHashCode() ?? 0;

    public override string ToString() =>
        $"{this.Guid} - {this.Key}";

    private string GetDebuggerDisplay() =>
        this.ToString();
}

public static class ClaimViewModelExtensions
{
    [return: NotNull]
    public static IEnumerable<string> ToSecurityKeys(this IEnumerable<ClaimViewModel> claims) =>
        claims.Select(x => x.Key).Compact();
}