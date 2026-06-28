namespace TimezoneCli.Domain;

public sealed record TimeComparison(
    ResolvedPlace RequestedPlace,
    ResolvedPlace ComparisonPlace,
    TimeSpan SignedTimeDifference,
    WorkingHoursAssessment RequestedWorkingHours,
    WorkingHoursAssessment ComparisonWorkingHours,
    bool CombinedSuitability,
    WorkingHoursWindow WorkingHoursWindow);
