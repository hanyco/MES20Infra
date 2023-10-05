#nullable disable

using System.Collections;
using System.Runtime.CompilerServices;

using Library.CodeGeneration.Models;

namespace Contracts.ViewModels;

/// <summary>
/// This class represents a collection of codes for various functionalities. It implements
/// IEnumerable&lt;Codes&gt; and is intended to be used as part of a source generator. The generated
/// codes will be returned using this class. Once generated, these codes will be utilized in a CQRS
/// code structure.
/// </summary>
public sealed class FunctionalityViewModelCodes : IEnumerable<Codes>
{
    private readonly Dictionary<string, Codes> _allCodes = new();

    /// <summary>
    /// Gets or sets the codes for the BlazorDetailsComponentViewModel functionality.
    /// </summary>
    public Codes BlazorDetailsComponentCodes { get => this.Get(); set => this.Set(value); }

    public Codes BlazorDetailsPageCodes { get => this.Get(); set => this.Set(value); }

    public Codes BlazorDetailsPageDataContextCodes { get => this.Get(); set => this.Set(value); }

    /// <summary>
    /// Gets or sets the codes for the BlazorListCodes functionality.
    /// </summary>
    public Codes BlazorListComponentCodes { get => this.Get(); set => this.Set(value); }

    public Codes BlazorListPageCodes { get => this.Get(); set => this.Set(value); }

    public Codes BlazorListPageDataContextCodes { get => this.Get(); set => this.Set(value); }

    /// <summary>
    /// Gets or sets the codes for the DeleteCommandCodes functionality.
    /// </summary>
    public Codes DeleteCommandCodes { get => this.Get(); set => this.Set(value); }

    /// <summary>
    /// Gets or sets the codes for the GetAllQueryCodes functionality.
    /// </summary>
    public Codes GetAllQueryCodes { get => this.Get(); set => this.Set(value); }

    /// <summary>
    /// Gets or sets the codes for the GetByIdQueryCodes functionality.
    /// </summary>
    public Codes GetByIdQueryCodes { get => this.Get(); set => this.Set(value); }

    /// <summary>
    /// Gets or sets the codes for the InsertCommandCodes functionality.
    /// </summary>
    public Codes InsertCommandCodes { get => this.Get(); set => this.Set(value); }

    public Codes SourceDtoCodes { get => this.Get(); set => this.Set(value); }
    
    public Codes ConverterCodes { get => this.Get(); set => this.Set(value); }

    /// <summary>
    /// Gets or sets the codes for the UpdateCommandCodes functionality.
    /// </summary>
    public Codes UpdateCommandCodes { get => this.Get(); set => this.Set(value); }

    public IEnumerable<Codes> GetAllCodes() =>
        this._allCodes.Select(x => x.Value).Compact();

    public IEnumerator<Codes> GetEnumerator() =>
        this.GetAllCodes().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    public Codes ToCodes() =>
        new(this.GetAllCodes());

    // Private method to get the codes associated with a functionality.
    private Codes Get([CallerMemberName] string propName = null) => this._allCodes[propName];

    // Private method to set the codes associated with a functionality.
    private void Set(Codes value, [CallerMemberName] string propName = null) =>
        this._allCodes[propName] = value;
}