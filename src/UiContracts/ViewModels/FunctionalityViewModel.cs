#nullable disable

using System.Collections;
using System.Runtime.CompilerServices;

using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Results;

namespace Contracts.ViewModels;

public sealed class FunctionalityViewModel : InfraViewModelBase
{
    private string _nameSpace;
    private DtoViewModel _sourceDto;

    public UiComponentViewModel BlazorDetailsComponentViewModel { get; set; }
    public DtoViewModel BlazorDetailsViewModel { get; set; }
    public UiComponentViewModel BlazorListComponentViewModel { get; set; }
    public DtoViewModel BlazorListViewModel { get; set; }
    public FunctionalityViewModelCodes Codes { get; } = new();

    public CqrsCommandViewModel DeleteCommandViewModel { get; set; }
    public CqrsQueryViewModel GetAllQueryViewModel { get; set; }
    public CqrsQueryViewModel GetByIdQueryViewModel { get; set; }
    public CqrsCommandViewModel InsertCommandViewModel { get; set; }
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
    public DtoViewModel SourceDto { get => this._sourceDto; set => this.SetProperty(ref this._sourceDto, value); }
    public CqrsCommandViewModel UpdateCommandViewModel { get; set; }
}

public sealed class FunctionalityViewModelCodes : IEnumerable<Codes>
{
    private readonly Dictionary<string, Codes> _allCodes = new();

    public Codes BlazorDetailsComponentViewModel { get => this.get(); set => this.set(value); }
    public Codes BlazorListCodes { get => this.get(); set => this.set(value); }
    public Codes DeleteCommandCodes { get => this.get(); set => this.set(value); }
    public Codes GetAllQueryCodes { get => this.get(); set => this.set(value); }
    public Codes GetByIdQueryCodes { get => this.get(); set => this.set(value); }
    public Codes InsertCommandCodes { get => this.get(); set => this.set(value); }
    public Codes UpdateCommandCodes { get => this.get(); set => this.set(value); }

    public IEnumerator<Codes> GetEnumerator() =>
        this._allCodes.Select(x => x.Value).Compact().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    private Codes get([CallerMemberName] string propName = null) =>
        this._allCodes[propName];

    private void set(Codes value, [CallerMemberName] string propName = null) =>
        this._allCodes[propName] = value;
}