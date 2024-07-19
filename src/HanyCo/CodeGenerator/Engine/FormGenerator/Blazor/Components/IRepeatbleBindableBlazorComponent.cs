namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components
{
    public interface IRepeatbleBindableBlazorComponent : IBindableBlazorComponent
    {
        void PlaceIteratorStatementBegin(string iteratorStartStatement);

        void PlaceIteratorStatementEnd(string iteratorEndStatement);
    }
}
