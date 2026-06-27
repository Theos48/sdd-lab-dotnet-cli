using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed class AliasCatalogTests
{
    [Fact]
    public void Validate_accepts_seed_alias_catalog()
    {
        var catalog = PlaceAliasCatalog.LoadDefault();

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_rejects_duplicate_normalized_alias()
    {
        var catalog = new AliasCatalog
        {
            Version = "1.0.0",
            Aliases =
            [
                Alias("city", "America/Mexico_City"),
                Alias("city", "America/New_York"),
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_rejects_missing_display_name()
    {
        var catalog = new AliasCatalog
        {
            Version = "1.0.0",
            Aliases =
            [
                new SupportedAlias
                {
                    Alias = "city",
                    NormalizedAlias = "city",
                    DisplayName = "",
                    TimeZoneId = "America/Mexico_City",
                },
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("display name", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_rejects_invalid_timezone_identifier()
    {
        var catalog = new AliasCatalog
        {
            Version = "1.0.0",
            Aliases =
            [
                Alias("city", "Not/AZone"),
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("invalid", StringComparison.Ordinal));
    }

    private static SupportedAlias Alias(string alias, string timeZoneId) =>
        new()
        {
            Alias = alias,
            NormalizedAlias = PlaceAliasCatalog.Normalize(alias),
            DisplayName = alias,
            TimeZoneId = timeZoneId,
        };
}
