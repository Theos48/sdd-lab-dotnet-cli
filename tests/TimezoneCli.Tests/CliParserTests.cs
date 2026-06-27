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
}
