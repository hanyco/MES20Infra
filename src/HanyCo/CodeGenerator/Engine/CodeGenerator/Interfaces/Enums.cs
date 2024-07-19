namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

[Flags]
public enum Partials
{
    None = 0,
    OnInitialize = 2,
    Handller = 4,
    Full = OnInitialize | Handller,
}
