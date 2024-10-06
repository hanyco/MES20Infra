namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public interface BackElement;

public interface FrontElement;

public interface IUiComponentContent;

public interface IUiComponentCqrsContent : IUiComponentContent
{
    CqrsViewModelBase? CqrsSegregate { get; set; }
}

public interface IUiComponentCustomContent : IUiComponentContent
{
    string? CodeStatement { get; set; }
}