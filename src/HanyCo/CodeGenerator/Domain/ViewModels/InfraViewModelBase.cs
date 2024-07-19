namespace HanyCo.Infra.CodeGen.Contracts.ViewModels;

public abstract class InfraViewModelBase() : InfraViewModelBase<long?>
{
    protected InfraViewModelBase(long? id, string? name) : this() =>
        (this.Id, this.Name) = (id, name);

    public static InfraViewModelBase NewEmpty(string name) =>
        new EmptyViewModel(null, name);

    private sealed class EmptyViewModel(long? id, string? name) : InfraViewModelBase(id, name)
    {
    }
}