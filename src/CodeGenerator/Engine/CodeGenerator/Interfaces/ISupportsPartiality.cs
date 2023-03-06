namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

public interface ISupportsPartiality
{
    Partials Partials { get; set; }

    Partials GetPartials();

    Partials GetValidPartials();
}
