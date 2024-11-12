using System.ComponentModel;

namespace Application.Common.Enums;

public enum AccessLevel
{
    None, // (Full Access)
    [Description("No Access")]
    NoAccess,
    [Description("Read Only")]
    ReadOnly
}

public static class AccessLevelHelper
{
    /// <summary>
    /// Gets the description of an AccessLevel value.
    /// </summary>
    public static string GetDescription(this AccessLevel value)
    {
        var field = value.GetType().GetField(value.ToString())!;
        var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                             .FirstOrDefault() as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Maps a string AccessType to its corresponding AccessLevel enum value.
    /// Returns AccessLevel.None if no match is found.
    /// </summary>
    public static AccessLevel MapAccessType(string accessType) =>
        Enum.GetValues(typeof(AccessLevel))
            .Cast<AccessLevel>()
            .FirstOrDefault(e => e.GetDescription().Equals(accessType, StringComparison.OrdinalIgnoreCase));
}
