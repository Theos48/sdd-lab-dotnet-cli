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

    [Fact]
    public void Resolve_rejects_mexican_postal_code_as_unsupported()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve("01000");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.UnsupportedMexicanPostalCode, result.Error!.Kind);
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
            Alias = alias,
            NormalizedAlias = PlaceAliasCatalog.Normalize(alias),
            DisplayName = displayName,
            TimeZoneId = timeZoneId,
        };
}
