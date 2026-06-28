using TimezoneCli.Cli;
using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed class CliParserTests
{
    [Fact]
    public void Parse_requires_place()
    {
        var result = CliParser.Parse([]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.MissingPlace, result.Error!.Kind);
    }

    [Fact]
    public void Parse_accepts_place_and_compare()
    {
        var result = CliParser.Parse(["--place", "America/Mexico_City", "--compare", "Europe/London"]);

        Assert.True(result.IsSuccess);
        Assert.Equal("America/Mexico_City", result.Value!.Place);
        Assert.Equal("Europe/London", result.Value.Compare);
    }

    [Fact]
    public void Parse_accepts_equals_form()
    {
        var result = CliParser.Parse(["--place=America/Mexico_City", "--compare=Europe/London"]);

        Assert.True(result.IsSuccess);
        Assert.Equal("America/Mexico_City", result.Value!.Place);
        Assert.Equal("Europe/London", result.Value.Compare);
        Assert.Null(result.Value.WorkingHoursWindow);
    }

    [Fact]
    public void Parse_accepts_custom_working_hours()
    {
        var result = CliParser.Parse(
            [
                "--place",
                "America/Mexico_City",
                "--compare",
                "Europe/London",
                "--working-hours-start",
                "08:30",
                "--working-hours-end",
                "16:45",
            ]);

        Assert.True(result.IsSuccess);
        Assert.Equal(new TimeOnly(8, 30), result.Value!.WorkingHoursWindow!.Start);
        Assert.Equal(new TimeOnly(16, 45), result.Value.WorkingHoursWindow.End);
    }

    [Fact]
    public void Parse_accepts_custom_working_hours_equals_form()
    {
        var result = CliParser.Parse(
            [
                "--place=America/Mexico_City",
                "--compare=Europe/London",
                "--working-hours-start=08:30",
                "--working-hours-end=16:45",
            ]);

        Assert.True(result.IsSuccess);
        Assert.Equal("08:30-16:45", result.Value!.WorkingHoursWindow!.Format());
    }

    [Fact]
    public void Parse_omits_working_hours_when_not_provided()
    {
        var result = CliParser.Parse(["--place", "America/Mexico_City", "--compare", "Europe/London"]);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.WorkingHoursWindow);
    }

    [Fact]
    public void Parse_rejects_too_many_comparison_values()
    {
        var result = CliParser.Parse(["--place", "America/Mexico_City", "--compare", "Europe/London", "--compare", "Asia/Tokyo"]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.TooManyComparisonPlaces, result.Error!.Kind);
    }

    [Fact]
    public void Parse_rejects_extra_positional_comparison_value_as_too_many_comparisons()
    {
        var result = CliParser.Parse(["--place", "America/Mexico_City", "--compare", "Europe/London", "Asia/Tokyo"]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.TooManyComparisonPlaces, result.Error!.Kind);
    }

    [Fact]
    public void Parse_rejects_unknown_argument()
    {
        var result = CliParser.Parse(["--place", "America/Mexico_City", "--calendar"]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.InvalidTimezoneIdentifier, result.Error!.Kind);
    }

    [Theory]
    [InlineData("--working-hours-start", "08:30")]
    [InlineData("--working-hours-end", "16:45")]
    public void Parse_rejects_missing_working_hours_pair(string flag, string value)
    {
        var result = CliParser.Parse(
            ["--place", "America/Mexico_City", "--compare", "Europe/London", flag, value]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.MissingWorkingHoursPair, result.Error!.Kind);
    }

    [Fact]
    public void Parse_rejects_working_hours_without_comparison()
    {
        var result = CliParser.Parse(
            [
                "--place",
                "America/Mexico_City",
                "--working-hours-start",
                "08:30",
                "--working-hours-end",
                "16:45",
            ]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.WorkingHoursWithoutComparison, result.Error!.Kind);
    }

    [Theory]
    [InlineData("9am")]
    [InlineData("24:00")]
    [InlineData("17:60")]
    public void Parse_rejects_malformed_working_hours_values(string start)
    {
        var result = CliParser.Parse(
            [
                "--place",
                "America/Mexico_City",
                "--compare",
                "Europe/London",
                "--working-hours-start",
                start,
                "--working-hours-end",
                "17:00",
            ]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.InvalidWorkingHoursTime, result.Error!.Kind);
        Assert.Equal(start, result.Error.Input);
    }

    [Theory]
    [InlineData("17:00", "17:00")]
    [InlineData("17:00", "09:00")]
    public void Parse_rejects_invalid_working_hours_ranges(string start, string end)
    {
        var result = CliParser.Parse(
            [
                "--place",
                "America/Mexico_City",
                "--compare",
                "Europe/London",
                "--working-hours-start",
                start,
                "--working-hours-end",
                end,
            ]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.InvalidWorkingHoursRange, result.Error!.Kind);
    }

    [Fact]
    public void Parse_prioritizes_invalid_working_hours_over_missing_place()
    {
        var result = CliParser.Parse(
            ["--working-hours-start", "bad", "--working-hours-end", "17:00"]);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.WorkingHoursWithoutComparison, result.Error!.Kind);
    }
}
