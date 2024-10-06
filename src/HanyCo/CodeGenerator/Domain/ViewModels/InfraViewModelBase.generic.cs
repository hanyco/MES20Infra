using System.Diagnostics;

using Library.ComponentModel;
using Library.Data.Markers;
using Library.Wpf.Bases;
using Library.Wpf.Markers;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
[ViewModel]
public abstract class InfraViewModelBase<TId> : NotifyPropertyChanged, IViewModel, IEquatable<InfraViewModelBase<TId>>, ICanSetKey<TId>, IEntity
{
    protected string? _name;

    private string? _description;

    private Guid? _guid;

    private TId? _id;

    protected InfraViewModelBase()
    {
    }

    protected InfraViewModelBase(TId id) => this.Id = id;

    protected InfraViewModelBase(string name, TId id)
        : this(id) => this.Name = name;

    public virtual string? Description
    {
        get => this._description;
        set => this.SetProperty(ref this._description, value);
    }

    public Guid? Guid
    {
        get => this._guid;
        set => this.SetProperty(ref this._guid, value);
    }

    public TId Id
    {
        get => this._id;
        set => this.SetProperty(ref this._id, value);
    }

    public virtual string? Name
    {
        get => this._name;
        set => this.SetProperty(ref this._name, value);
    }

    public ISet<ClaimViewModel> SecurityClaims { get; } = new HashSet<ClaimViewModel>();

    public static bool operator !=(InfraViewModelBase<TId>? left, InfraViewModelBase<TId>? right) =>
            !(left == right);

    public static bool operator ==(InfraViewModelBase<TId>? left, InfraViewModelBase<TId>? right) =>
        left is null && right is null || left is not null && right is not null && left.Equals(right);

    public virtual bool Equals(InfraViewModelBase<TId>? other) =>
        other is not null && (this.Id, other.Id) switch
        {
            (null, null) => this.Name == other.Name,
            (not null, null) => false,
            (null, not null) => false,
            (_, _) => this.Id.Equals(other.Id)
        };

    public override bool Equals(object? obj) =>
        this.Equals(obj as InfraViewModelBase<TId>);

    public override int GetHashCode() =>
        HashCode.Combine(this.Id, this.Name);

    public override string ToString() =>
        this.Id is not null ? $"{this.Name} ({this.Id})" : $"{this.Name}";

    private string GetDebuggerDisplay() =>
        this.ToString();
}