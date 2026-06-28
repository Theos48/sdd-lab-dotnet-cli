namespace TimezoneCli.Domain;

public sealed class SupportedAlias
{
    public const string AliasCategory = "alias";

    public const string MexicanPostalCodeCategory = "mexican-postal-code";

    public string Input { get; init; } = string.Empty;

    public string NormalizedInput { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string TimeZoneId { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;
}
