using Library.Results;

namespace Library.Validations.Policies;

/// <summary>
/// String policies.
/// </summary>
public static class StringPolicies
{
    /// <summary>
    /// Contains digit
    /// </summary>
    public static StringPolicy ContainsDigit => value => value.Any(char.IsDigit);
    /// <summary>
    /// Contains lower case
    /// </summary>
    public static StringPolicy ContainsLowerCase => value => value.Any(char.IsLower);
    /// <summary>
    /// Contains special character
    /// </summary>
    public static StringPolicy ContainsSpecialCharacter => value => value.Any(char.IsPunctuation) || value.Any(char.IsSymbol);
    /// <summary>
    /// Contains upper case
    /// </summary>
    public static StringPolicy ContainsUpperCase => value => value.Any(char.IsUpper);
    /// <summary>
    /// Is alphanumeric
    /// </summary>
    public static StringPolicy IsAlphanumeric => value => value.All(char.IsLetterOrDigit);
    /// <summary>
    /// Is digit
    /// </summary>
    public static StringPolicy IsDigit => value => value.All(char.IsDigit);
    /// <summary>
    /// Is letter
    /// </summary>
    public static StringPolicy IsLetter => value => value.All(char.IsLetter);
    /// <summary>
    /// Is letter or digit
    /// </summary>
    public static StringPolicy IsLetterOrDigit => value => value.All(char.IsLetterOrDigit);
    /// <summary>
    /// Is lower case
    /// </summary>
    public static StringPolicy IsLowerCase => value => value.All(char.IsLower);
    /// <summary>
    /// Is upper case
    /// </summary>
    public static StringPolicy IsUpperCase => value => value.All(char.IsUpper);
    /// <summary>
    /// No constraint.
    /// </summary>
    public static StringPolicy NoConstraint => _ => true;
    /// <summary>
    /// Not null or empty.
    /// </summary>
    public static StringPolicy NotNullOrEmpty => value => !string.IsNullOrEmpty(value);
    /// <summary>
    /// Default password policies.
    /// </summary>
    public static IEnumerable<StringPolicy> PasswordPolicies => [ContainsDigit, ContainsLowerCase, ContainsUpperCase, ContainsSpecialCharacter, AtLeast(8)];
    /// <summary>
    /// At least the specified length.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static StringPolicy AtLeast(int length) => value => value.Length >= length;
    /// <summary>
    /// Contains any of the specified characters.
    /// </summary>
    /// <param name="characters"></param>
    /// <returns></returns>
    public static StringPolicy ContainsAny(params IEnumerable<char> characters) => value => value.Any(characters.Contains);

    /// <summary>
    /// Validate the specified value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IResult<IEnumerable<StringPolicy>> Validate(string value, params IEnumerable<StringPolicy> policies) =>
        policies.ArgumentNotNull().Where(policy => !policy(value)) switch
        {
            var failedPolicies when failedPolicies.Any() => Result.Fail(failedPolicies),
            _ => Result.Success<IEnumerable<StringPolicy>>()
        };

    /// <summary>
    /// Validate the specified password.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static IResult<IEnumerable<StringPolicy>> ValidatePassword(string password) =>
        Validate(password, PasswordPolicies);

    /// <summary>
    /// The main delegate for string policy.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool StringPolicy(string value);
}
