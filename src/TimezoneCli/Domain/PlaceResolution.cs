namespace TimezoneCli.Domain;

public sealed record PlaceResolution(
    string DisplayName,
    string TimeZoneId,
    ResolvedPlaceSource Source);
