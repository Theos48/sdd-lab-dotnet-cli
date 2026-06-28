namespace TimezoneCli.Domain;

public enum ResolvedPlaceSource
{
    TimezoneIdentifier,
    SupportedAlias,
    SupportedMexicanPostalCode,
}

public sealed record ResolvedPlace(
    string DisplayName,
    string TimeZoneId,
    DateOnly LocalDate,
    TimeOnly LocalTime,
    TimeSpan UtcOffset,
    ResolvedPlaceSource Source);
