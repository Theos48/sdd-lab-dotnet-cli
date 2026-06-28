using System.Text.Json;
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
                    Input = "city",
                    NormalizedInput = "city",
                    DisplayName = "",
                    TimeZoneId = "America/Mexico_City",
                    Category = SupportedAlias.AliasCategory,
                },
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("display name", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_rejects_missing_required_entry_fields()
    {
        var catalog = new AliasCatalog
        {
            Version = "1.0.0",
            Aliases =
            [
                new SupportedAlias(),
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("input is required", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("normalized input is required", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("display name", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("category", StringComparison.Ordinal));
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

    [Fact]
    public void Validate_rejects_unsupported_category()
    {
        var catalog = new AliasCatalog
        {
            Version = "1.0.0",
            Aliases =
            [
                new SupportedAlias
                {
                    Input = "city",
                    NormalizedInput = "city",
                    DisplayName = "city",
                    TimeZoneId = "America/Mexico_City",
                    Category = "region",
                },
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("unsupported", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123456")]
    [InlineData("12A45")]
    public void Validate_rejects_malformed_mexican_postal_code(string input)
    {
        var catalog = new AliasCatalog
        {
            Version = "1.0.0",
            Aliases =
            [
                MexicanPostalCode(input, "America/Mexico_City"),
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("malformed", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_rejects_alias_that_looks_like_mexican_postal_code()
    {
        var catalog = new AliasCatalog
        {
            Version = "1.0.0",
            Aliases =
            [
                Alias("01000", "America/Mexico_City"),
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("must not look like", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_preserves_leading_zero_postal_codes_as_strings()
    {
        var catalog = new AliasCatalog
        {
            Version = "1.0.0",
            Aliases =
            [
                MexicanPostalCode("01000", "America/Mexico_City"),
            ],
        };

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Empty(errors);
        Assert.Equal("01000", catalog.Aliases[0].Input);
        Assert.Equal("01000", catalog.Aliases[0].NormalizedInput);
    }

    [Fact]
    public void Validate_rejects_legacy_alias_catalog_shape()
    {
        const string json = """
            {
              "version": "1.0.0",
              "aliases": [
                {
                  "alias": "mexico city",
                  "normalizedAlias": "mexico city",
                  "displayName": "Mexico City",
                  "timeZoneId": "America/Mexico_City"
                }
              ]
            }
            """;

        var catalog = JsonSerializer.Deserialize<AliasCatalog>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var errors = PlaceAliasCatalog.Validate(catalog);

        Assert.Contains(errors, error => error.Contains("input is required", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("normalized input is required", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("category", StringComparison.Ordinal));
    }

    private static SupportedAlias Alias(string input, string timeZoneId) =>
        new()
        {
            Input = input,
            NormalizedInput = PlaceAliasCatalog.Normalize(input),
            DisplayName = input,
            TimeZoneId = timeZoneId,
            Category = SupportedAlias.AliasCategory,
        };

    private static SupportedAlias MexicanPostalCode(string input, string timeZoneId) =>
        new()
        {
            Input = input,
            NormalizedInput = PlaceAliasCatalog.Normalize(input),
            DisplayName = input,
            TimeZoneId = timeZoneId,
            Category = SupportedAlias.MexicanPostalCodeCategory,
        };
}
