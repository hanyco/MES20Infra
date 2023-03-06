namespace HanyCo.Infra.UI.ViewModels;

public abstract class InfraViewModelBase : InfraViewModelBase<long?>
{
    protected InfraViewModelBase()
    {
    }

    protected InfraViewModelBase(long? id, string? name)
        => (this.Id, this.Name) = (id, name);

    private sealed class EmptyViewModel : InfraViewModelBase
    {
        public EmptyViewModel(long? id, string? name) : base(id, name)
        {
        }
    }

    public static InfraViewModelBase NewEmpty(string name) 
        => new EmptyViewModel(null, name);
}
