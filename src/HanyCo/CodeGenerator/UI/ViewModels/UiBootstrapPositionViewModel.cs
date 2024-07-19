using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using Library.Mapping;
using Library.Validations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.UI.ViewModels;

/// <summary>
/// <seealso cref="HanyCo.Infra.UI.ViewModels.InfraViewModelBase" />
/// <seealso cref="System.IComparable" />
/// <seealso cref="System.IComparable{HanyCo.Infra.UI.ViewModels.UiBootstrapPositionViewModel}" />
[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class UiBootstrapPositionViewModel : InfraViewModelBase, IComparable, IComparable<UiBootstrapPositionViewModel>
{
    #region Fields

    private int? _Col;
    private int? _ColSpan;
    private int? _Order;
    private int? _Row;
    private int? _RowSpan;

    #endregion

    /// <summary>
    /// Gets or sets the col.
    /// </summary>
    /// <value>
    /// The col.
    /// </value>
    public int? Col { get => this._Col; set => this.SetProperty(ref this._Col, value); }

    /// <summary>
    /// Gets or sets the col span.
    /// </summary>
    /// <value>
    /// The col span.
    /// </value>
    public int? ColSpan { get => this._ColSpan; set => this.SetProperty(ref this._ColSpan, value); }

    /// <summary>
    /// Gets or sets the order.
    /// </summary>
    /// <value>
    /// The order.
    /// </value>
    public int? Order { get => this._Order; set => this.SetProperty(ref this._Order, value); }

    /// <summary>
    /// Gets or sets the row.
    /// </summary>
    /// <value>
    /// The row.
    /// </value>
    public int? Row { get => this._Row; set => this.SetProperty(ref this._Row, value); }

    /// <summary>
    /// Gets or sets the row span.
    /// </summary>
    /// <value>
    /// The row span.
    /// </value>
    public int? RowSpan { get => this._RowSpan; set => this.SetProperty(ref this._RowSpan, value); }

    public UiBootstrapPositionViewModel FillWith([DisallowNull] UiBootstrapPositionViewModel positionViewModel)
    {
        Check.NotNull(positionViewModel);
        var mapper = new Mapper();
        mapper.Map(positionViewModel, this);
        //this.Order = positionViewModel.Order;
        //this.Row = positionViewModel.Row;
        //this.Col = positionViewModel.Col;
        //this.RowSpan = positionViewModel.RowSpan;
        //this.ColSpan = positionViewModel.ColSpan;
        return this;
    }

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

    /// <summary>
    /// Equalses the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj switch
        {
            null => false,
            _ => this.CompareTo(obj) == 0
        };

    public override int GetHashCode()
        => HashCode.Combine(this.Order, this.Row, this.Col);

    /// <summary>
    /// Converts to bootstrapposition.
    /// </summary>
    /// <returns></returns>
    public BootstrapPosition ToBootstrapPosition()
        => new()
        {
            Order = this.Order,
            Row = this.Row,
            Col = this.Col,
            ColSpan = this.ColSpan
        };

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => (this.Order, this.Row, this.Col) switch
        {
            (null, null, null) => "(default)",
            (not null, _, _) => $"Order: {this.Order}",
            (_, not null, null) => $"({this.Row}, null)",
            (_, null, not null) => $"(invalid)",
            _ => $"({this.Row},{this.Col})"
        };

    /// <summary>
    /// Gets the debugger display.
    /// </summary>
    /// <returns></returns>
    private string GetDebuggerDisplay()
        => this.ToString();


    public static bool operator ==(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
                => left is null ? right is null : left.Equals(right);

    public static bool operator !=(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => !(left == right);

    public static bool operator <(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(UiBootstrapPositionViewModel left, UiBootstrapPositionViewModel right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
}
