using System.Text.Json;

namespace TimezoneCli.Domain;

public static class PlaceAliasCatalog
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static AliasCatalog LoadDefault()
    {
        var outputPath = Path.Combine(AppContext.BaseDirectory, "Data", "place-aliases.json");
        if (File.Exists(outputPath))
        {
            return LoadFromFile(outputPath);
        }

        var sourcePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "src",
            "TimezoneCli",
            "Data",
            "place-aliases.json");
        return LoadFromFile(sourcePath);
    }

    public static AliasCatalog LoadFromFile(string path)
    {
        using var stream = File.OpenRead(path);
        var catalog = JsonSerializer.Deserialize<AliasCatalog>(stream, JsonOptions)
            ?? throw new InvalidOperationException("Alias catalog could not be read.");

        var errors = Validate(catalog);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"Alias catalog is invalid: {string.Join("; ", errors)}");
        }

        return catalog;
    }

    public static IReadOnlyList<string> Validate(AliasCatalog catalog, bool allowAmbiguous = false)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(catalog.Version))
        {
            errors.Add("version is required");
        }

        var timezoneResolver = new TimezoneResolver();
        foreach (var alias in catalog.Aliases)
        {
            if (string.IsNullOrWhiteSpace(alias.Alias))
            {
                errors.Add("alias is required");
            }

            if (string.IsNullOrWhiteSpace(alias.NormalizedAlias))
            {
                errors.Add($"normalized alias is required for '{alias.Alias}'");
            }

            if (Normalize(alias.Alias) != alias.NormalizedAlias)
            {
                errors.Add($"normalized alias mismatch for '{alias.Alias}'");
            }

            if (string.IsNullOrWhiteSpace(alias.DisplayName))
            {
                errors.Add($"display name is required for '{alias.Alias}'");
            }

            if (timezoneResolver.TryResolve(alias.TimeZoneId) is null)
            {
                errors.Add($"timezone '{alias.TimeZoneId}' is invalid for '{alias.Alias}'");
            }
        }

        if (!allowAmbiguous)
        {
            var duplicateKeys = catalog.Aliases
                .GroupBy(alias => alias.NormalizedAlias)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);

            foreach (var key in duplicateKeys)
            {
                errors.Add($"normalized alias '{key}' is duplicated");
            }
        }

        return errors;
    }

    public static string Normalize(string value) =>
        string.Join(
            ' ',
            value.Trim().ToLowerInvariant().Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}
