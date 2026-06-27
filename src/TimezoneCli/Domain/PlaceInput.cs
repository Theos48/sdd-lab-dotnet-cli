namespace TimezoneCli.Domain;

public enum PlaceInputKind
{
    TimezoneIdentifier,
    SupportedAlias,
    MexicanPostalCode,
    Unknown,
    Invalid,
}

public enum PlaceInputRole
{
    RequestedPlace,
    ComparisonPlace,
}

public sealed record PlaceInput(
    string RawValue,
    string NormalizedValue,
    PlaceInputKind InputKind,
    PlaceInputRole Role);
