using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TimezoneCli.Domain;

public static partial class PlaceAliasCatalog
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
            if (string.IsNullOrWhiteSpace(alias.Input))
            {
                errors.Add("input is required");
            }

            if (string.IsNullOrWhiteSpace(alias.NormalizedInput))
            {
                errors.Add($"normalized input is required for '{alias.Input}'");
            }

            if (!string.IsNullOrWhiteSpace(alias.Input)
                && !string.IsNullOrWhiteSpace(alias.NormalizedInput)
                && Normalize(alias.Input) != alias.NormalizedInput)
            {
                errors.Add($"normalized input mismatch for '{alias.Input}'");
            }

            if (string.IsNullOrWhiteSpace(alias.DisplayName))
            {
                errors.Add($"display name is required for '{alias.Input}'");
            }

            if (timezoneResolver.TryResolve(alias.TimeZoneId) is null)
            {
                errors.Add($"timezone '{alias.TimeZoneId}' is invalid for '{alias.Input}'");
            }

            if (alias.Category is not SupportedAlias.AliasCategory and not SupportedAlias.MexicanPostalCodeCategory)
            {
                errors.Add($"category '{alias.Category}' is unsupported for '{alias.Input}'");
            }

            if (alias.Category == SupportedAlias.MexicanPostalCodeCategory
                && !MexicanPostalCodeRegex().IsMatch(alias.Input))
            {
                errors.Add($"mexican postal code '{alias.Input}' is malformed");
            }

            if (alias.Category == SupportedAlias.AliasCategory
                && MexicanPostalCodeRegex().IsMatch(alias.Input))
            {
                errors.Add($"alias '{alias.Input}' must not look like a Mexican postal code");
            }
        }

        if (!allowAmbiguous)
        {
            var duplicateKeys = catalog.Aliases
                .GroupBy(alias => alias.NormalizedInput)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);

            foreach (var key in duplicateKeys)
            {
                errors.Add($"normalized input '{key}' is duplicated");
            }
        }

        return errors;
    }

    public static string Normalize(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new System.Text.StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(
                char.IsWhiteSpace(character) || char.IsPunctuation(character) || char.IsSeparator(character)
                    ? ' '
                    : character);
        }

        return RepeatedWhitespaceRegex()
            .Replace(builder.ToString().Normalize(NormalizationForm.FormC), " ")
            .Trim();
    }

    [GeneratedRegex("^\\d{5}$", RegexOptions.CultureInvariant)]
    private static partial Regex MexicanPostalCodeRegex();

    [GeneratedRegex("\\s+", RegexOptions.CultureInvariant)]
    private static partial Regex RepeatedWhitespaceRegex();
}
