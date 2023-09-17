#nullable disable

using System.Collections;
using System.Runtime.CompilerServices;

using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;

namespace Contracts.ViewModels;

/// <summary>
/// This class represents a functionality-specific view model. It inherits from InfraViewModelBase
/// and is designed to store various components and codes related to a specific functionality. This
/// class is closely related to the FunctionalityViewModelCodes class for storing
/// functionality-specific codes.
/// </summary>
public sealed class FunctionalityViewModel : InfraViewModelBase
{
    private DtoViewModel _sourceDto; // Private field to store the source DTO associated with the functionality.

    // Components and view models associated with the BlazorDetailsComponent functionality.
    public UiComponentViewModel BlazorDetailsComponentViewModel { get; set; }

    // Components and view models associated with the BlazorList functionality.
    public UiComponentViewModel BlazorListComponentViewModel { get; set; }

    public UiPageViewModel BlazorListPageViewModel { get; set; }
    public UiPageViewModel BlazorDetailsPageViewModel { get; set; }

    // An instance of FunctionalityViewModelCodes to store functionality-specific codes.
    public FunctionalityViewModelCodes Codes { get; } = new();

    // Components and view models associated with various CQRS commands and queries.
    public CqrsCommandViewModel DeleteCommandViewModel { get; set; }

    public CqrsQueryViewModel GetAllQueryViewModel { get; set; }
    public CqrsQueryViewModel GetByIdQueryViewModel { get; set; }
    public CqrsCommandViewModel InsertCommandViewModel { get; set; }

    // Property to get or set the namespace of the functionality.
    //public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }

    // Property to get or set the source DTO associated with the functionality.
    public DtoViewModel SourceDto { get => this._sourceDto; set => this.SetProperty(ref this._sourceDto, value); }

    public CqrsCommandViewModel UpdateCommandViewModel { get; set; }
}

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

    /// <summary>
    /// Gets or sets the codes for the BlazorListCodes functionality.
    /// </summary>
    public Codes BlazorListComponentCodes { get => this.Get(); set => this.Set(value); }

    public Codes BlazorPageCodes { get => this.Get(); set => this.Set(value); }

    public Codes BlazorPageDataContextCodes { get => this.Get(); set => this.Set(value); }

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

    /// <summary>
    /// Gets or sets the codes for the UpdateCommandCodes functionality.
    /// </summary>
    public Codes UpdateCommandCodes { get => this.Get(); set => this.Set(value); }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of codes.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<Codes> GetEnumerator() =>
        this._allCodes.Select(x => x.Value).Compact().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    // Private method to get the codes associated with a functionality.
    private Codes Get([CallerMemberName] string propName = null) =>
        this._allCodes[propName];

    // Private method to set the codes associated with a functionality.
    private void Set(Codes value, [CallerMemberName] string propName = null) =>
        this._allCodes[propName] = value;
}