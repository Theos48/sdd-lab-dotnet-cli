using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed class AliasResolverTests
{
    [Fact]
    public void Resolve_accepts_supported_alias()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve("mexico city");

        Assert.True(result.IsSuccess);
        Assert.Equal("Mexico City", result.Value!.DisplayName);
        Assert.Equal("America/Mexico_City", result.Value.TimeZoneId);
        Assert.Equal(ResolvedPlaceSource.SupportedAlias, result.Value.Source);
    }

    [Theory]
    [InlineData("mexico city", "Mexico City", "America/Mexico_City")]
    [InlineData("london", "London", "Europe/London")]
    [InlineData("new york", "New York", "America/New_York")]
    [InlineData("tokyo", "Tokyo", "Asia/Tokyo")]
    public void Resolve_accepts_all_seed_aliases(string input, string displayName, string timeZoneId)
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(displayName, result.Value!.DisplayName);
        Assert.Equal(timeZoneId, result.Value.TimeZoneId);
        Assert.Equal(ResolvedPlaceSource.SupportedAlias, result.Value.Source);
    }

    [Theory]
    [InlineData("  MEXICO   CITY  ")]
    [InlineData("MEXICO---City")]
    [InlineData("México City")]
    public void Resolve_normalizes_supported_alias_input(string input)
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(input);

        Assert.True(result.IsSuccess);
        Assert.Equal("Mexico City", result.Value!.DisplayName);
        Assert.Equal("America/Mexico_City", result.Value.TimeZoneId);
    }

    [Fact]
    public void Resolve_accepts_supported_mexican_postal_code()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve("01000");

        Assert.True(result.IsSuccess);
        Assert.Equal("01000", result.Value!.DisplayName);
        Assert.Equal("America/Mexico_City", result.Value.TimeZoneId);
        Assert.Equal(ResolvedPlaceSource.SupportedMexicanPostalCode, result.Value.Source);
    }

    [Theory]
    [InlineData("01000", "America/Mexico_City")]
    [InlineData("64000", "America/Monterrey")]
    [InlineData("44100", "America/Mexico_City")]
    public void Resolve_accepts_all_supported_mexican_postal_codes(string input, string timeZoneId)
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(input, result.Value!.DisplayName);
        Assert.Equal(timeZoneId, result.Value.TimeZoneId);
        Assert.Equal(ResolvedPlaceSource.SupportedMexicanPostalCode, result.Value.Source);
    }

    [Fact]
    public void Resolve_rejects_unsupported_mexican_postal_code()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve("99999");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.UnsupportedMexicanPostalCode, result.Error!.Kind);
        Assert.Equal("99999", result.Error.Input);
    }

    [Fact]
    public void Resolve_rejects_unknown_place()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve("Atlantis");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.UnknownPlace, result.Error!.Kind);
    }

    [Fact]
    public void Resolve_rejects_misspelled_alias_without_fuzzy_matching()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve("mexico citty");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.UnknownPlace, result.Error!.Kind);
    }

    [Fact]
    public void Resolve_rejects_invalid_timezone_identifier()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve("America/Atlantis");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.InvalidTimezoneIdentifier, result.Error!.Kind);
    }

    [Fact]
    public void Resolve_rejects_ambiguous_alias_with_known_matches()
    {
        var resolver = new AliasResolver(
            new AliasCatalog
            {
                Version = "1.0.0",
                Aliases =
                [
                    Alias("city", "City Mexico", "America/Mexico_City"),
                    Alias("city", "City New York", "America/New_York"),
                ],
            },
            new TimezoneResolver());

        var result = resolver.Resolve("city");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal(ResolutionErrorKind.AmbiguousPlace, result.Error!.Kind);
        Assert.NotEmpty(result.Error.KnownMatches);
    }

    [Fact]
    public void Ambiguous_error_can_have_no_safe_matches()
    {
        var error = ResolutionError.AmbiguousPlace("city");

        Assert.Equal(ResolutionErrorKind.AmbiguousPlace, error.Kind);
        Assert.Empty(error.KnownMatches);
    }

    private static AliasResolver CreateResolver() =>
        new(PlaceAliasCatalog.LoadDefault(), new TimezoneResolver());

    private static SupportedAlias Alias(string alias, string displayName, string timeZoneId) =>
        new()
        {
            Input = alias,
            NormalizedInput = PlaceAliasCatalog.Normalize(alias),
            DisplayName = displayName,
            TimeZoneId = timeZoneId,
            Category = SupportedAlias.AliasCategory,
        };
}
