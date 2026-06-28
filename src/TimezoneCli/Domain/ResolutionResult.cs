namespace TimezoneCli.Domain;

public enum ResolutionErrorKind
{
    MissingPlace,
    InvalidTimezoneIdentifier,
    UnknownPlace,
    UnsupportedMexicanPostalCode,
    AmbiguousPlace,
    TooManyComparisonPlaces,
    MissingWorkingHoursPair,
    WorkingHoursWithoutComparison,
    InvalidWorkingHoursTime,
    InvalidWorkingHoursRange,
}

public sealed record ResolutionError(
    ResolutionErrorKind Kind,
    string? Input = null,
    IReadOnlyList<string>? Matches = null)
{
    public IReadOnlyList<string> KnownMatches { get; } = Matches ?? [];

    public static ResolutionError MissingPlace() =>
        new(ResolutionErrorKind.MissingPlace);

    public static ResolutionError InvalidTimezoneIdentifier(string input) =>
        new(ResolutionErrorKind.InvalidTimezoneIdentifier, input);

    public static ResolutionError InvalidArgument(string input) =>
        new(ResolutionErrorKind.InvalidTimezoneIdentifier, input);

    public static ResolutionError UnknownPlace(string input) =>
        new(ResolutionErrorKind.UnknownPlace, input);

    public static ResolutionError UnsupportedMexicanPostalCode(string input) =>
        new(ResolutionErrorKind.UnsupportedMexicanPostalCode, input);

    public static ResolutionError AmbiguousPlace(string input, IReadOnlyList<string>? matches = null) =>
        new(ResolutionErrorKind.AmbiguousPlace, input, matches);

    public static ResolutionError TooManyComparisonPlaces() =>
        new(ResolutionErrorKind.TooManyComparisonPlaces);

    public static ResolutionError MissingWorkingHoursPair() =>
        new(ResolutionErrorKind.MissingWorkingHoursPair);

    public static ResolutionError WorkingHoursWithoutComparison() =>
        new(ResolutionErrorKind.WorkingHoursWithoutComparison);

    public static ResolutionError InvalidWorkingHoursTime(string input) =>
        new(ResolutionErrorKind.InvalidWorkingHoursTime, input);

    public static ResolutionError InvalidWorkingHoursRange() =>
        new(ResolutionErrorKind.InvalidWorkingHoursRange);
}

public sealed record ResolutionResult<T>(T? Value, ResolutionError? Error)
{
    public bool IsSuccess => Error is null;

    public static ResolutionResult<T> Success(T value) =>
        new(value, null);

    public static ResolutionResult<T> Failure(ResolutionError error) =>
        new(default, error);
}
