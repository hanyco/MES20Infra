using System.Diagnostics;

using Library.Data.Markers;
using Library.Validations;

namespace Library.Mapping;

[DebuggerStepThrough]
public static class MapperExtensions
{
    public static TEntity ForMember<TEntity>(this TEntity entity, in Action<TEntity> action)
        where TEntity : IEntity
    {
        Check.MustBeArgumentNotNull(action);
        action(entity);
        return entity;
    }
}