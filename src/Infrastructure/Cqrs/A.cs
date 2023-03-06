using Library.Cqrs.Models.Queries;

namespace HanyCo.Infra.Cqrs;

[Obsolete("Just for internal uses.")]
public interface IQueryParameter
{
}
public interface IQueryParameter<TQueryResult> : IQuery<TQueryResult>
{
}

public interface IQueryResult: Library.Cqrs.Models.Queries.IQueryResult
{
    
}

public interface IQueryResult<TResult> : Library.Cqrs.Models.Queries.IQueryResult<TResult>
{

}