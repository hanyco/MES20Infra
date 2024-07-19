using System.Diagnostics.CodeAnalysis;

using Library.Validations;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

[Immutable]
public sealed record BootstrapPosition : IEquatable<BootstrapPosition>, IEqualityComparer<BootstrapPosition>, IComparable<BootstrapPosition>, IComparable
{
#pragma warning disable IDE0032 // Use auto property
    private int? _col;
    private int? _colSpan;
    private int? _offset;
    private int? _row;
#pragma warning restore IDE0032 // Use auto property

    public BootstrapPosition(int? order = null, int? isRow = null, int? col = null, int? colSpan = null, int? offset = null)
    {
        this.Order = order;
        this.Row = isRow;
        this.Col = col;
        this.ColSpan = colSpan;
        this.Offset = offset;
    }

    public int? Order { get; init; }
    public int? Row { get => _row; init => _row = value; }
    public int? Col { get => _col; init => _col = value; }
    public int? ColSpan { get => _colSpan; init => _colSpan = value; }
    public int? Offset { get => _offset; init => _offset = value; }

    public bool IsInitialized => this.Order is not null || this.Row is not null || this.Col is not null || this.ColSpan is not null;

    public BootstrapPosition SetColSpan(int? value)
        => this.Fluent<BootstrapPosition>(() => this._colSpan = value);
    public BootstrapPosition SetCol(int? value)
        => this.Fluent<BootstrapPosition>(() => this._col = value);
    public int CompareTo(BootstrapPosition? other)
    {
        if (other is { } o)
        {
            if (compare(this.Order, o.Order) is { } rOrder)
            {
                return rOrder;
            }
            else if (this.Row != o.Row)
            {
                return 1;
            }
            else if (compare(this.Col, o.Col) is { } rCol)
            {
                return rCol;
            }

            return 0;
        }
        else
        {
            return 1;
        }

        static int? compare(int? thisProp, int? oProp)
            => thisProp != oProp ? oProp ?? 0 - oProp ?? 0 : null;
    }

    public int CompareTo(object? obj)
        => this.CompareTo(obj.Cast().As<BootstrapPosition>());

    public bool Equals(BootstrapPosition? other)
        => this.CompareTo(other) == 0;

    public bool Equals(BootstrapPosition? x, BootstrapPosition? y)
        => (x is null && y is null) || x?.CompareTo(y) == 0;

    public override int GetHashCode()
        => ObjectHelper.GetHashCode(this, this.Order ?? -1, this.Row??-1, this.Col ?? -1);

    public int GetHashCode([DisallowNull] BootstrapPosition obj)
        => obj.ArgumentNotNull(nameof(obj)).GetHashCode();

    public override string? ToString()
        => !this.IsInitialized ? null : $"Order: {this.Order}, Location:({this.Row},{this.Col}), ColSpan:{this.ColSpan}";

    public BootstrapPosition SetRow(int? row) =>
        this.Fluent(this._row = row);

    public static bool operator <(BootstrapPosition left, BootstrapPosition right)
        => left.ArgumentNotNull().CompareTo(right.ArgumentNotNull()) < 0;

    public static bool operator <=(BootstrapPosition left, BootstrapPosition right)
        => left.ArgumentNotNull().CompareTo(right.ArgumentNotNull()) <= 0;

    public static bool operator >(BootstrapPosition left, BootstrapPosition right)
        => left.ArgumentNotNull().CompareTo(right.ArgumentNotNull()) > 0;

    public static bool operator >=(BootstrapPosition left, BootstrapPosition right)
        => left.ArgumentNotNull().CompareTo(right.ArgumentNotNull()) >= 0;

    public bool IsDefault()
        => !this.IsInitialized;
}