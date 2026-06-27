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
        var normalized = PlaceAliasCatalog.Normalize(trimmed);

        if (MexicanPostalCodeRegex().IsMatch(normalized))
        {
            return ResolutionResult<PlaceResolution>.Failure(
                ResolutionError.UnsupportedMexicanPostalCode(trimmed));
        }

        var timeZone = timezoneResolver.TryResolve(trimmed);
        if (timeZone is not null)
        {
            return ResolutionResult<PlaceResolution>.Success(
                new PlaceResolution(timeZone.Id, timeZone.Id, ResolvedPlaceSource.TimezoneIdentifier));
        }

        var matches = catalog.Aliases
            .Where(alias => alias.NormalizedAlias == normalized)
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
                    ResolvedPlaceSource.SupportedAlias));
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
