using System.Collections;

using Library.Results;
using Library.Validations;

namespace Library.Validations.Rules;

/// <summary>
/// String policies.
/// </summary>
public class StringValidationRules : IValidationRules<string>
{
    private readonly List<Func<string, bool>> _rules = [];

    /// <summary>
    /// Contains digit
    /// </summary>
    public static Func<string, bool> ContainsDigit => value => value.Any(char.IsDigit);

    /// <summary>
    /// Contains lower case
    /// </summary>
    public static Func<string, bool> ContainsLowerCase => value => value.Any(char.IsLower);

    /// <summary>
    /// Contains special character
    /// </summary>
    public static Func<string, bool> ContainsSpecialCharacter => value => value.Any(char.IsPunctuation) || value.Any(char.IsSymbol);

    /// <summary>
    /// Contains upper case
    /// </summary>
    public static Func<string, bool> ContainsUpperCase => value => value.Any(char.IsUpper);

    /// <summary>
    /// Is alphanumeric
    /// </summary>
    public static Func<string, bool> IsAlphanumeric => value => value.All(char.IsLetterOrDigit);

    /// <summary>
    /// Is digit
    /// </summary>
    public static Func<string, bool> IsDigit => value => value.All(char.IsDigit);

    /// <summary>
    /// Is letter
    /// </summary>
    public static Func<string, bool> IsLetter => value => value.All(char.IsLetter);

    /// <summary>
    /// Is letter or digit
    /// </summary>
    public static Func<string, bool> IsLetterOrDigit => value => value.All(char.IsLetterOrDigit);

    /// <summary>
    /// Is lower case
    /// </summary>
    public static Func<string, bool> IsLowerCase => value => value.All(char.IsLower);

    /// <summary>
    /// Is upper case
    /// </summary>
    public static Func<string, bool> IsUpperCase => value => value.All(char.IsUpper);

    /// <summary>
    /// No constraint.
    /// </summary>
    public static Func<string, bool> NoConstraint => _ => true;

    /// <summary>
    /// Not null or empty.
    /// </summary>
    public static Func<string, bool> NotNullOrEmpty => value => !string.IsNullOrEmpty(value);

    /// <summary>
    /// Default password policies.
    /// </summary>
    public static IEnumerable<Func<string, bool>> PasswordPolicies => [ContainsDigit, ContainsLowerCase, ContainsUpperCase, ContainsSpecialCharacter, AtLeast(8)];

    /// <summary>
    /// At least the specified length.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static Func<string, bool> AtLeast(int length) => value => value.Length >= length;

    /// <summary>
    /// Contains any of the specified characters.
    /// </summary>
    /// <param name="characters"></param>
    /// <returns></returns>
    public static Func<string, bool> ContainsAny(params IEnumerable<char> characters) => value => value.Any(characters.Contains);

    /// <summary>
    /// Validate the specified value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IResult<IEnumerable<Func<string, bool>>> Validate(string value, params IEnumerable<Func<string, bool>> policies) =>
        policies.ArgumentNotNull().Where(policy => !policy(value)) switch
        {
            var failedPolicies when failedPolicies.Any() => Result.Fail(failedPolicies),
            _ => Result.Success<IEnumerable<Func<string, bool>>>()
        };

    /// <summary>
    /// Validate the specified password.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static IResult<IEnumerable<Func<string, bool>>> ValidatePassword(string password) =>
        Validate(password, PasswordPolicies);

    /// <summary>
    /// Adds a rule.
    /// </summary>
    /// <param name="item"></param>
    public void Add(Func<string, bool> item) =>
        this._rules.Add(item);

    /// <summary>
    /// Clear all rules.
    /// </summary>
    public void Clear() => this._rules.Clear();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Func<string, bool>> GetEnumerator() =>
        this._rules.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

}
