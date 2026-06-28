using System.Text.RegularExpressions;

namespace TimezoneCli.Domain;

public sealed partial class AliasResolver(AliasCatalog catalog, TimezoneResolver timezoneResolver)
{
    public ResolutionResult<PlaceResolution> Resolve(string rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            return ResolutionResult<PlaceResolution>.Failure(
                ResolutionError.InvalidTimezoneIdentifier(rawInput));
        }

        var trimmed = rawInput.Trim();
        var timeZone = timezoneResolver.TryResolve(trimmed);
        if (timeZone is not null)
        {
            return ResolutionResult<PlaceResolution>.Success(
                new PlaceResolution(timeZone.Id, timeZone.Id, ResolvedPlaceSource.TimezoneIdentifier));
        }

        var normalized = PlaceAliasCatalog.Normalize(trimmed);
        var matches = catalog.Aliases
            .Where(alias => alias.NormalizedInput == normalized)
            .ToList();

        if (matches.Count == 1)
        {
            var alias = matches[0];
            var aliasTimeZone = timezoneResolver.TryResolve(alias.TimeZoneId);
            if (aliasTimeZone is null)
            {
                return ResolutionResult<PlaceResolution>.Failure(
                    ResolutionError.InvalidTimezoneIdentifier(alias.TimeZoneId));
            }

            return ResolutionResult<PlaceResolution>.Success(
                new PlaceResolution(
                    alias.DisplayName,
                    aliasTimeZone.Id,
                    alias.Category == SupportedAlias.MexicanPostalCodeCategory
                        ? ResolvedPlaceSource.SupportedMexicanPostalCode
                        : ResolvedPlaceSource.SupportedAlias));
        }

        if (matches.Count > 1)
        {
            var knownMatches = matches
                .Select(alias => $"{alias.DisplayName} ({alias.TimeZoneId})")
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            return ResolutionResult<PlaceResolution>.Failure(
                ResolutionError.AmbiguousPlace(trimmed, knownMatches));
        }

        if (MexicanPostalCodeRegex().IsMatch(trimmed))
        {
            return ResolutionResult<PlaceResolution>.Failure(
                ResolutionError.UnsupportedMexicanPostalCode(trimmed));
        }

        if (trimmed.Contains('/', StringComparison.Ordinal))
        {
            return ResolutionResult<PlaceResolution>.Failure(
                ResolutionError.InvalidTimezoneIdentifier(trimmed));
        }

        return ResolutionResult<PlaceResolution>.Failure(
            ResolutionError.UnknownPlace(trimmed));
    }

    [GeneratedRegex("^\\d{5}$", RegexOptions.CultureInvariant)]
    private static partial Regex MexicanPostalCodeRegex();
}
