using System.Text.RegularExpressions;
using TimezoneCli.Cli;
using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed partial class CliResultWriterTests
{
    [Fact]
    public void Lookup_writes_exact_labels_and_value_formats()
    {
        var result = CliResultWriter.Lookup(Place("Mexico City", "America/Mexico_City", new DateOnly(2026, 6, 27), new TimeOnly(9, 30), TimeSpan.FromHours(-6)));

        Assert.Equal(ExitCodes.Success, result.ExitCode);
        Assert.Empty(result.StderrLines);
        Assert.Equal("Place: Mexico City", result.StdoutLines[0]);
        Assert.Equal("Timezone: America/Mexico_City", result.StdoutLines[1]);
        Assert.Matches(DateLineRegex(), result.StdoutLines[2]);
        Assert.Matches(TimeLineRegex(), result.StdoutLines[3]);
        Assert.Matches(OffsetLineRegex(), result.StdoutLines[4]);
    }

    [Fact]
    public void Lookup_preserves_place_and_timezone_labels_for_supported_alias()
    {
        var result = CliResultWriter.Lookup(
            Place(
                "Mexico City",
                "America/Mexico_City",
                new DateOnly(2026, 6, 27),
                new TimeOnly(9, 30),
                TimeSpan.FromHours(-6),
                ResolvedPlaceSource.SupportedAlias));

        Assert.Contains("Place: Mexico City", result.StdoutLines);
        Assert.Contains("Timezone: America/Mexico_City", result.StdoutLines);
    }

    [Fact]
    public void Lookup_preserves_place_and_timezone_labels_for_supported_postal_code()
    {
        var result = CliResultWriter.Lookup(
            Place(
                "01000",
                "America/Mexico_City",
                new DateOnly(2026, 6, 27),
                new TimeOnly(9, 30),
                TimeSpan.FromHours(-6),
                ResolvedPlaceSource.SupportedMexicanPostalCode));

        Assert.Contains("Place: 01000", result.StdoutLines);
        Assert.Contains("Timezone: America/Mexico_City", result.StdoutLines);
    }

    [Fact]
    public void Comparison_writes_exact_labels_and_value_formats()
    {
        var comparison = new TimeComparison(
            Place("Mexico City", "America/Mexico_City", new DateOnly(2026, 1, 15), new TimeOnly(9, 0), TimeSpan.FromHours(-6)),
            Place("London", "Europe/London", new DateOnly(2026, 1, 15), new TimeOnly(15, 0), TimeSpan.Zero),
            TimeSpan.FromHours(6),
            new WorkingHoursAssessment(true, "weekday-within-hours"),
            new WorkingHoursAssessment(true, "weekday-within-hours"),
            true,
            WorkingHoursWindow.Default);

        var result = CliResultWriter.Comparison(comparison);

        Assert.Equal(ExitCodes.Success, result.ExitCode);
        Assert.Contains("Working hours: within", result.StdoutLines);
        Assert.Contains("Working hours window: 09:00-17:00", result.StdoutLines);
        Assert.Contains("Compared working hours: within", result.StdoutLines);
        Assert.Contains("Time difference: +6:00", result.StdoutLines);
        Assert.Contains("Meeting suitability: suitable", result.StdoutLines);
    }

    [Fact]
    public void Comparison_preserves_place_and_timezone_labels_for_supported_aliases()
    {
        var comparison = new TimeComparison(
            Place("Mexico City", "America/Mexico_City", new DateOnly(2026, 1, 15), new TimeOnly(9, 0), TimeSpan.FromHours(-6), ResolvedPlaceSource.SupportedAlias),
            Place("London", "Europe/London", new DateOnly(2026, 1, 15), new TimeOnly(15, 0), TimeSpan.Zero, ResolvedPlaceSource.SupportedAlias),
            TimeSpan.FromHours(6),
            new WorkingHoursAssessment(true, "weekday-within-hours"),
            new WorkingHoursAssessment(true, "weekday-within-hours"),
            true,
            WorkingHoursWindow.Default);

        var result = CliResultWriter.Comparison(comparison);

        Assert.Contains("Place: Mexico City", result.StdoutLines);
        Assert.Contains("Timezone: America/Mexico_City", result.StdoutLines);
        Assert.Contains("Compared place: London", result.StdoutLines);
        Assert.Contains("Compared timezone: Europe/London", result.StdoutLines);
    }

    [Fact]
    public void Comparison_writes_custom_working_hours_window()
    {
        var comparison = new TimeComparison(
            Place("Mexico City", "America/Mexico_City", new DateOnly(2026, 1, 15), new TimeOnly(8, 30), TimeSpan.FromHours(-6)),
            Place("London", "Europe/London", new DateOnly(2026, 1, 15), new TimeOnly(14, 30), TimeSpan.Zero),
            TimeSpan.FromHours(6),
            new WorkingHoursAssessment(true, "weekday-within-hours"),
            new WorkingHoursAssessment(true, "weekday-within-hours"),
            true,
            new WorkingHoursWindow(new TimeOnly(8, 30), new TimeOnly(16, 45)));

        var result = CliResultWriter.Comparison(comparison);

        Assert.Contains("Working hours window: 08:30-16:45", result.StdoutLines);
    }

    [Theory]
    [InlineData(ResolutionErrorKind.MissingPlace, null, ExitCodes.InvalidInput, "Error: --place is required.")]
    [InlineData(ResolutionErrorKind.InvalidTimezoneIdentifier, "Bad/Zone", ExitCodes.InvalidInput, "Error: invalid timezone identifier 'Bad/Zone'.")]
    [InlineData(ResolutionErrorKind.UnknownPlace, "Atlantis", ExitCodes.UnknownInput, "Error: unknown place 'Atlantis'.")]
    [InlineData(ResolutionErrorKind.UnsupportedMexicanPostalCode, "99999", ExitCodes.UnsupportedInput, "Error: Mexican postal code '99999' is not supported in v1.")]
    [InlineData(ResolutionErrorKind.TooManyComparisonPlaces, null, ExitCodes.InvalidInput, "Error: only one comparison place is supported in v1.")]
    [InlineData(ResolutionErrorKind.MissingWorkingHoursPair, null, ExitCodes.InvalidInput, "Error: --working-hours-start and --working-hours-end must be provided together.")]
    [InlineData(ResolutionErrorKind.WorkingHoursWithoutComparison, null, ExitCodes.InvalidInput, "Error: working-hours options require --compare.")]
    [InlineData(ResolutionErrorKind.MissingWorkingHoursValue, "--working-hours-start", ExitCodes.InvalidInput, "Error: --working-hours-start requires a value.")]
    [InlineData(ResolutionErrorKind.InvalidWorkingHoursTime, "bad", ExitCodes.InvalidInput, "Error: invalid working-hours time 'bad'.")]
    [InlineData(ResolutionErrorKind.InvalidWorkingHoursRange, null, ExitCodes.InvalidInput, "Error: --working-hours-end must be later than --working-hours-start.")]
    public void Error_maps_message_and_exit_code(
        ResolutionErrorKind kind,
        string? input,
        int exitCode,
        string firstLine)
    {
        var result = CliResultWriter.Error(new ResolutionError(kind, input));

        Assert.Equal(exitCode, result.ExitCode);
        Assert.Empty(result.StdoutLines);
        Assert.Equal(firstLine, result.StderrLines[0]);
    }

    [Fact]
    public void Error_lists_known_matches_for_ambiguous_input()
    {
        var result = CliResultWriter.Error(
            ResolutionError.AmbiguousPlace("city", ["City A (America/Mexico_City)", "City B (America/New_York)"]));

        Assert.Equal(ExitCodes.AmbiguousInput, result.ExitCode);
        Assert.Contains("Known matches: City A (America/Mexico_City), City B (America/New_York)", result.StderrLines);
    }

    [Fact]
    public void Error_lists_supported_mexican_postal_codes_for_unsupported_postal_code()
    {
        var result = CliResultWriter.Error(ResolutionError.UnsupportedMexicanPostalCode("99999"));

        Assert.Equal(ExitCodes.UnsupportedInput, result.ExitCode);
        Assert.Contains("Supported Mexican postal codes: 01000, 64000, 44100", result.StderrLines);
        Assert.Contains("Use an IANA timezone such as America/Mexico_City.", result.StderrLines);
    }

    private static ResolvedPlace Place(
        string displayName,
        string timeZoneId,
        DateOnly localDate,
        TimeOnly localTime,
        TimeSpan utcOffset,
        ResolvedPlaceSource source = ResolvedPlaceSource.TimezoneIdentifier) =>
        new(displayName, timeZoneId, localDate, localTime, utcOffset, source);

    [GeneratedRegex("^Local date: \\d{4}-\\d{2}-\\d{2}$")]
    private static partial Regex DateLineRegex();

    [GeneratedRegex("^Local time: \\d{2}:\\d{2}$")]
    private static partial Regex TimeLineRegex();

    [GeneratedRegex("^UTC offset: [+-]\\d{2}:\\d{2}$")]
    private static partial Regex OffsetLineRegex();
}
