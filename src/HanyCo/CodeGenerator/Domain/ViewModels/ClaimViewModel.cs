using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ClaimViewModel(string? key, object? value, Guid? id, ClaimViewModel? parent)
    : InfraViewModelBase<Guid>(id ?? System.Guid.NewGuid()), IEquatable<ClaimViewModel>
{
    public ClaimViewModel()
        : this(null, null, null, null) { }

    public ClaimViewModel(string? key, object? value)
        : this(key, value, null, null) { }

    public ClaimViewModel(string? key, object? value, Guid? id)
        : this(key, value, id, null) { }

    public ClaimViewModel(string? key, object? value, ClaimViewModel? parent)
        : this(key, value, null, parent) { }

    public string? Key { get; set; } = key;

    public ClaimViewModel? Parent { get; set; } = parent;

    public object? Value { get; set; } = value;

    public static bool operator !=(ClaimViewModel left, ClaimViewModel right) =>
        !Equals(left, right);

    public static bool operator ==(ClaimViewModel left, ClaimViewModel right) =>
        Equals(left, right);

    public bool Equals(ClaimViewModel? other) =>
        (other?.GetHashCode() ?? 0) == this.GetHashCode();

    public override bool Equals(object? obj) =>
        this.Equals(obj as ClaimViewModel);

    public override int GetHashCode() =>
        this.Key?.GetHashCode() ?? 0;

    public override string ToString() =>
        $"{this.Guid} - {this.Key}=\"{this.Value ?? "(null)"}\"";

    private string GetDebuggerDisplay() =>
        this.ToString();
}

public static class ClaimViewModelExtensions
{
    [return: NotNull]
    public static IEnumerable<string> ToSecurityKeys(this IEnumerable<ClaimViewModel> claims) =>
        claims.Select(x => x.Key).Compact();
}