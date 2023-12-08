using Library.CodeGeneration.v2.Back;

namespace Contracts.ViewModels;

public sealed class EditFormInfo
{
    public HashSet<EditFormEventInfo> Events { get; } = [];
    public bool IsEditForm { get; set; }
    public string Model { get; set; } = "DataContext";
}

public readonly struct EditFormEventInfo(string eventName, Method eventHandler, bool isPartial = true) : IEquatable<EditFormEventInfo>
{
    public IMethod Handler { get; } = eventHandler;
    public bool IsPartial { get; } = isPartial;
    public string Name { get; } = eventName;

    public static bool operator !=(EditFormEventInfo left, EditFormEventInfo right) =>
        !(left == right);

    public static bool operator ==(EditFormEventInfo left, EditFormEventInfo right) =>
        left.Equals(right);

    public override bool Equals(object obj) =>
        obj is EditFormEventInfo other && this.Equals(other);

    public bool Equals(EditFormEventInfo other) =>
        other.GetHashCode() == this.GetHashCode();

    public override int GetHashCode() =>
        this.Name.GetHashCode();
}