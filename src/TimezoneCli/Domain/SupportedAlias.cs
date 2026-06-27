namespace TimezoneCli.Domain;

public sealed class SupportedAlias
{
    public string Alias { get; init; } = string.Empty;

    public string NormalizedAlias { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string TimeZoneId { get; init; } = string.Empty;
}
