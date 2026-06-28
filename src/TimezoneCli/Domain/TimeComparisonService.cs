namespace TimezoneCli.Domain;

public sealed class TimeComparisonService(
    AliasResolver aliasResolver,
    TimeProvider clock,
    WorkingHoursPolicy workingHoursPolicy)
{
    public ResolutionResult<ResolvedPlace> Lookup(string placeInput)
    {
        var resolved = aliasResolver.Resolve(placeInput);
        if (!resolved.IsSuccess)
        {
            return ResolutionResult<ResolvedPlace>.Failure(resolved.Error!);
        }

        return ResolutionResult<ResolvedPlace>.Success(CreateResolvedPlace(resolved.Value!));
    }

    public ResolutionResult<TimeComparison> Compare(
        string requestedInput,
        string comparisonInput,
        WorkingHoursWindow? workingHoursWindow = null)
    {
        var activeWindow = workingHoursWindow ?? WorkingHoursWindow.Default;
        var requested = Lookup(requestedInput);
        if (!requested.IsSuccess)
        {
            return ResolutionResult<TimeComparison>.Failure(requested.Error!);
        }

        var comparison = Lookup(comparisonInput);
        if (!comparison.IsSuccess)
        {
            return ResolutionResult<TimeComparison>.Failure(comparison.Error!);
        }

        var requestedWorkingHours = workingHoursPolicy.Assess(requested.Value!, activeWindow);
        var comparisonWorkingHours = workingHoursPolicy.Assess(comparison.Value!, activeWindow);
        var signedTimeDifference = comparison.Value!.UtcOffset - requested.Value!.UtcOffset;

        return ResolutionResult<TimeComparison>.Success(
            new TimeComparison(
                requested.Value!,
                comparison.Value!,
                signedTimeDifference,
                requestedWorkingHours,
                comparisonWorkingHours,
                requestedWorkingHours.IsWithinWorkingHours && comparisonWorkingHours.IsWithinWorkingHours,
                activeWindow));
    }

    private ResolvedPlace CreateResolvedPlace(PlaceResolution resolution)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(resolution.TimeZoneId);
        var local = TimeZoneInfo.ConvertTime(clock.GetUtcNow(), timeZone);
        return new ResolvedPlace(
            resolution.DisplayName,
            timeZone.Id,
            DateOnly.FromDateTime(local.DateTime),
            TimeOnly.FromDateTime(local.DateTime),
            local.Offset,
            resolution.Source);
    }
}
