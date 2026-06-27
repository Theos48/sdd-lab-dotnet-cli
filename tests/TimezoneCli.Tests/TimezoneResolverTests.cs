using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed class TimezoneResolverTests
{
    [Fact]
    public void ResolveIdentifier_accepts_iana_timezone()
    {
        var resolver = new TimezoneResolver();

        var result = resolver.ResolveIdentifier("America/Mexico_City");

        Assert.True(result.IsSuccess);
        Assert.Equal("America/Mexico_City", result.Value!.TimeZoneId);
        Assert.Equal("America/Mexico_City", result.Value.DisplayName);
        Assert.Equal(ResolvedPlaceSource.TimezoneIdentifier, result.Value.Source);
    }

    [Fact]
    public void ResolveIdentifier_rejects_invalid_timezone()
    {
        var resolver = new TimezoneResolver();

        var result = resolver.ResolveIdentifier("America/Nope");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.InvalidTimezoneIdentifier, result.Error!.Kind);
    }
}
