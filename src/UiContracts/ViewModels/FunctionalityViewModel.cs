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
    public UiComponentViewModel BlazorListComponentViewModel { get; set; }

    public DtoViewModel BlazorDetailsViewModel { get; set; }
    public DtoViewModel BlazorListViewModel { get; set; }
    public FunctionalityViewModelCodesResults CodesResults { get; } = new();

    public CqrsCommandViewModel DeleteCommandViewModel { get; set; }
    public CqrsQueryViewModel GetAllQueryViewModel { get; set; }
    public CqrsQueryViewModel GetByIdQueryViewModel { get; set; }
    public CqrsCommandViewModel InsertCommandViewModel { get; set; }
    public string NameSpace { get => this._nameSpace; set => this.SetProperty(ref this._nameSpace, value); }
    public DtoViewModel SourceDto { get => this._sourceDto; set => this.SetProperty(ref this._sourceDto, value); }
    public CqrsCommandViewModel UpdateCommandViewModel { get; set; }
}

public sealed class FunctionalityViewModelCodesResults : IEnumerable<Result<Codes>>
{
    private readonly Dictionary<string, Result<Codes>> _allCodes = new();

    public Result<Codes> BlazorDetailsComponentViewModel { get => this.get(); set => this.set(value); }
    public Result<Codes> BlazorListCodes { get => this.get(); set => this.set(value); }
    public Result<Codes> DeleteCommandCodes { get => this.get(); set => this.set(value); }
    public Result<Codes> GetAllQueryCodes { get => this.get(); set => this.set(value); }
    public Result<Codes> GetByIdQueryCodes { get => this.get(); set => this.set(value); }
    public Result<Codes> InsertCommandCodes { get => this.get(); set => this.set(value); }
    public Result<Codes> UpdateCommandCodes { get => this.get(); set => this.set(value); }

    public IEnumerator<Result<Codes>> GetEnumerator() =>
        this._allCodes.Select(x => x.Value).Compact().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    private Result<Codes> get([CallerMemberName] string propName = null) =>
        this._allCodes[propName];

    private void set(Result<Codes> value, [CallerMemberName] string propName = null) =>
        this._allCodes[propName] = value;
}