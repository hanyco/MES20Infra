
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.Mapping;
using Library.Validations;

namespace Contracts.ViewModels;

/// <summary> <seealso cref="InfraViewModelBase" /> <seealso
/// cref="IComparable" /> <seealso
/// cref="IComparable{UiBootstrapPositionViewModel}" />
[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class UiBootstrapPositionViewModel : InfraViewModelBase, IComparable, IComparable<UiBootstrapPositionViewModel>
{
    #region Fields

    private int? _col;
    private int? _colSpan;
    private int? _order;
    private int? _row;
    private int? _rowSpan;
    private int? _offset;

    #endregion Fields

    /// <summary>
    /// Gets or sets the col.
    /// </summary>
    /// <value>The col.</value>
    public int? Col { get => this._col; set => this.SetProperty(ref this._col, value); }

    /// <summary>
    /// Gets or sets the col span.
    /// </summary>
    /// <value>The col span.</value>
    public int? ColSpan { get => this._colSpan; set => this.SetProperty(ref this._colSpan, value); }

    public int? Offset { get => this._offset; set => this.SetProperty(ref this._offset, value); }

    /// <summary>
    /// Gets or sets the order.
    /// </summary>
    /// <value>The order.</value>
    public int? Order { get => this._order; set => this.SetProperty(ref this._order, value); }

    /// <summary>
    /// Gets or sets the row.
    /// </summary>
    /// <value>The row.</value>
    public int? Row { get => this._row; set => this.SetProperty(ref this._row, value); }

    /// <summary>
    /// Gets or sets the row span.
    /// </summary>
    /// <value>The row span.</value>
    public int? RowSpan { get => this._rowSpan; set => this.SetProperty(ref this._rowSpan, value); }

    public static bool operator !=(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => !(left == right);

    public static bool operator <(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator ==(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
                => left is null ? right is null : left.Equals(right);

    public static bool operator >(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(UiBootstrapPositionViewModel? other)
    {
        if (other is null)
        {
            return -1;
        }

        var buffer = (this.Order, other.Order) switch
        {
            (not null, null) => 1,
            (null, not null) => -1,
            ({ } tO, { } oO) => (int?)tO.CompareTo(oO),
            _ => null
        };
        if (buffer is { } orderResult)
        {
            return orderResult;
        }

        buffer = (this.Row, other.Row) switch
        {
            (not null, null) => 1,
            (null, not null) => -1,
            ({ } tO, { } oO) => tO.CompareTo(oO),
            _ => null
        };
        if (buffer is { } rowBuffer and not 0)
        {
            return rowBuffer;
        }
        if (buffer is null)
        {
            return 0;
        }

        var result = (this.Col, other.Col) switch
        {
            (not null, null) => 1,
            (null, not null) => -1,
            ({ } tO, { } oO) => tO.CompareTo(oO),
            _ => 0
        };
        return result;
    }

    public int CompareTo(object? obj)
        => obj is not UiBootstrapPositionViewModel position ? -1 : this.CompareTo(position);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj switch
        {
            null => false,
            _ => this.CompareTo(obj) == 0
        };

    public UiBootstrapPositionViewModel FillWith([DisallowNull] UiBootstrapPositionViewModel positionViewModel)
    {
        Check.MutBeNotNull(positionViewModel);
        var mapper = new Mapper();
        _ = mapper.Map(positionViewModel, this);
        //this.Order = positionViewModel.Order;
        //this.Row = positionViewModel.Row;
        //this.Col = positionViewModel.Col;
        //this.RowSpan = positionViewModel.RowSpan;
        //this.ColSpan = positionViewModel.ColSpan;
        return this;
    }

    public override int GetHashCode()
        => HashCode.Combine(this.Order, this.Row, this.Col);

    /// <summary>
    /// Converts to bootstrap position.
    /// </summary>
    /// <returns></returns>
    public BootstrapPosition ToBootstrapPosition()
        => new()
        {
            Order = this.Order,
            Row = this.Row,
            Col = this.Col,
            ColSpan = this.ColSpan,
            Offset = this.Offset,
        };

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (NumberHelper.IsNullOrZero(this.Order, this.Row, this.Col, this.Offset))
        {
            return "(default)";
        }

        if (!this.Order.IsNullOrZero() && NumberHelper.IsNullOrZero(this.Row, this.Col, this.Offset))
        {
            return $"Order: {this.Order}";
        }

        if (!this.Order.IsNullOrZero())
        {
            return "(invalid)";
        }

        var result = new StringBuilder();
        if (!this.Row.IsNullOrZero())
        {
            _ = result.Append($" Row: {this.Row}");
        }

        if (!this.Col.IsNullOrZero())
        {
            _ = result.Append($" Col: {this.Col}");
        }

        if (!this.RowSpan.IsNullOrZero())
        {
            _ = result.Append($" Rowspan: {this.Col}");
        }

        if (!this.ColSpan.IsNullOrZero())
        {
            _ = result.Append($" Colspan: {this.Col}");
        }

        if (!this.Offset.IsNullOrZero())
        {
            _ = result.Append($" Offset: {this.Col}");
        }

        return $"({result.ToString().Trim()})";
    }

    /// <summary>
    /// Gets the debugger display.
    /// </summary>
    /// <returns></returns>
    private string GetDebuggerDisplay()
        => this.ToString();
}