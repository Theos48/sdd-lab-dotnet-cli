using System.Globalization;
using TimezoneCli.Domain;

namespace TimezoneCli.Cli;

public static class CliResultWriter
{
    public static CliResult Lookup(ResolvedPlace place) =>
        new(
            ExitCodes.Success,
            [
                $"Place: {place.DisplayName}",
                $"Timezone: {place.TimeZoneId}",
                $"Local date: {place.LocalDate:yyyy-MM-dd}",
                $"Local time: {place.LocalTime:HH\\:mm}",
                $"UTC offset: {FormatOffset(place.UtcOffset)}",
            ],
            []);

    public static CliResult Comparison(TimeComparison comparison) =>
        new(
            ExitCodes.Success,
            [
                $"Place: {comparison.RequestedPlace.DisplayName}",
                $"Timezone: {comparison.RequestedPlace.TimeZoneId}",
                $"Local date: {comparison.RequestedPlace.LocalDate:yyyy-MM-dd}",
                $"Local time: {comparison.RequestedPlace.LocalTime:HH\\:mm}",
                $"UTC offset: {FormatOffset(comparison.RequestedPlace.UtcOffset)}",
                $"Working hours: {FormatWorkingHours(comparison.RequestedWorkingHours)}",
                $"Working hours window: {comparison.WorkingHoursWindow.Format()}",
                string.Empty,
                $"Compared place: {comparison.ComparisonPlace.DisplayName}",
                $"Compared timezone: {comparison.ComparisonPlace.TimeZoneId}",
                $"Compared local date: {comparison.ComparisonPlace.LocalDate:yyyy-MM-dd}",
                $"Compared local time: {comparison.ComparisonPlace.LocalTime:HH\\:mm}",
                $"Compared UTC offset: {FormatOffset(comparison.ComparisonPlace.UtcOffset)}",
                $"Compared working hours: {FormatWorkingHours(comparison.ComparisonWorkingHours)}",
                $"Time difference: {FormatDifference(comparison.SignedTimeDifference)}",
                $"Meeting suitability: {(comparison.CombinedSuitability ? "suitable" : "not suitable")}",
            ],
            []);

    public static CliResult Error(ResolutionError error) =>
        new(
            ExitCodes.FromErrorKind(error.Kind),
            [],
            BuildErrorLines(error));

    public static string FormatOffset(TimeSpan offset)
    {
        var sign = offset < TimeSpan.Zero ? "-" : "+";
        var absolute = offset.Duration();
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{sign}{(int)absolute.TotalHours:00}:{absolute.Minutes:00}");
    }

    public static string FormatDifference(TimeSpan difference)
    {
        if (difference == TimeSpan.Zero)
        {
            return "0:00";
        }

        var sign = difference < TimeSpan.Zero ? "-" : "+";
        var absolute = difference.Duration();
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{sign}{(int)absolute.TotalHours}:{absolute.Minutes:00}");
    }

    private static IReadOnlyList<string> BuildErrorLines(ResolutionError error) =>
        error.Kind switch
        {
            ResolutionErrorKind.MissingPlace =>
            [
                "Error: --place is required.",
                "Example: --place America/Mexico_City",
            ],
            ResolutionErrorKind.InvalidTimezoneIdentifier when error.Input is not null =>
            [
                $"Error: invalid timezone identifier '{error.Input}'.",
                "Use an IANA timezone such as America/Mexico_City.",
            ],
            ResolutionErrorKind.UnknownPlace when error.Input is not null =>
            [
                $"Error: unknown place '{error.Input}'.",
                "Use an IANA timezone such as America/Mexico_City.",
            ],
            ResolutionErrorKind.UnsupportedMexicanPostalCode =>
            [
                "Error: Mexican postal codes are not supported in v1.",
                "Use an IANA timezone such as America/Mexico_City.",
            ],
            ResolutionErrorKind.AmbiguousPlace when error.KnownMatches.Count > 0 && error.Input is not null =>
            [
                $"Error: ambiguous place '{error.Input}'.",
                $"Known matches: {string.Join(", ", error.KnownMatches)}",
                "Use a more specific IANA timezone.",
            ],
            ResolutionErrorKind.AmbiguousPlace when error.Input is not null =>
            [
                $"Error: ambiguous place '{error.Input}'.",
                "Use a more specific IANA timezone.",
            ],
            ResolutionErrorKind.TooManyComparisonPlaces =>
            [
                "Error: only one comparison place is supported in v1.",
            ],
            ResolutionErrorKind.MissingWorkingHoursPair =>
            [
                "Error: --working-hours-start and --working-hours-end must be provided together.",
                "Example: --working-hours-start 09:00 --working-hours-end 17:00",
            ],
            ResolutionErrorKind.WorkingHoursWithoutComparison =>
            [
                "Error: working-hours options require --compare.",
            ],
            ResolutionErrorKind.InvalidWorkingHoursTime when error.Input is not null =>
            [
                $"Error: invalid working-hours time '{error.Input}'.",
                "Use HH:mm 24-hour format, for example 09:00.",
            ],
            ResolutionErrorKind.InvalidWorkingHoursRange =>
            [
                "Error: --working-hours-end must be later than --working-hours-start.",
                "Overnight and zero-length working-hours windows are not supported in v1.",
            ],
            _ =>
            [
                "Error: invalid input.",
                "Use an IANA timezone such as America/Mexico_City.",
            ],
        };

    private static string FormatWorkingHours(WorkingHoursAssessment assessment) =>
        assessment.IsWithinWorkingHours ? "within" : "outside";
}
