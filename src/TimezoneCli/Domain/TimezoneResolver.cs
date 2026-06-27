namespace TimezoneCli.Domain;

public sealed class TimezoneResolver
{
    public TimeZoneInfo? TryResolve(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId.Trim());
        }
        catch (TimeZoneNotFoundException)
        {
            return null;
        }
        catch (InvalidTimeZoneException)
        {
            return null;
        }
    }

    public ResolutionResult<PlaceResolution> ResolveIdentifier(string input)
    {
        var timeZone = TryResolve(input);
        if (timeZone is null)
        {
            return ResolutionResult<PlaceResolution>.Failure(
                ResolutionError.InvalidTimezoneIdentifier(input));
        }

        return ResolutionResult<PlaceResolution>.Success(
            new PlaceResolution(timeZone.Id, timeZone.Id, ResolvedPlaceSource.TimezoneIdentifier));
    }
}
