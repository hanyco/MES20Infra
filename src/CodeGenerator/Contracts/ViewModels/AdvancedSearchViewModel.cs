using System.Collections;

using Library.CodeGeneration;

namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public sealed class AdvancedSearchViewModel : IList<AdvancedSearchOperation>
{
    private readonly List<AdvancedSearchOperation> _operationList = [];
    public int Count => ((ICollection<AdvancedSearchOperation>)this._operationList).Count;
    public bool IsReadOnly => ((ICollection<AdvancedSearchOperation>)this._operationList).IsReadOnly;
    public AdvancedSearchOperation this[int index] { get => ((IList<AdvancedSearchOperation>)this._operationList)[index]; set => ((IList<AdvancedSearchOperation>)this._operationList)[index] = value; }

    public void Add(AdvancedSearchOperation item) => ((ICollection<AdvancedSearchOperation>)this._operationList).Add(item);

    public void Clear() => ((ICollection<AdvancedSearchOperation>)this._operationList).Clear();

    public bool Contains(AdvancedSearchOperation item) => ((ICollection<AdvancedSearchOperation>)this._operationList).Contains(item);

    public void CopyTo(AdvancedSearchOperation[] array, int arrayIndex) => ((ICollection<AdvancedSearchOperation>)this._operationList).CopyTo(array, arrayIndex);

    public IEnumerator<AdvancedSearchOperation> GetEnumerator() => ((IEnumerable<AdvancedSearchOperation>)this._operationList).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this._operationList).GetEnumerator();

    public int IndexOf(AdvancedSearchOperation item) => ((IList<AdvancedSearchOperation>)this._operationList).IndexOf(item);

    public void Insert(int index, AdvancedSearchOperation item) => ((IList<AdvancedSearchOperation>)this._operationList).Insert(index, item);

    public bool Remove(AdvancedSearchOperation item) => ((ICollection<AdvancedSearchOperation>)this._operationList).Remove(item);

    public void RemoveAt(int index) => ((IList<AdvancedSearchOperation>)this._operationList).RemoveAt(index);
}

public readonly record struct AdvancedSearchField(string Name, TypePath Type);
public readonly record struct AdvancedSearchOperation(AdvancedSearchField Field, AdvancedSearchFieldOperator Operator, IEnumerable<object> Parameters);

public enum AdvancedSearchFieldOperator
{
    IsBiggerThan,
    IsLessThan,
    Contains,
    StartsWith,
    EndsWith,
    Equals,
    NotEquals
}