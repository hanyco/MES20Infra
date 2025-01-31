namespace Library.Validations.Rules;

public interface IValidationRules<TValue> : IEnumerable<Func<TValue, bool>>
{
}
