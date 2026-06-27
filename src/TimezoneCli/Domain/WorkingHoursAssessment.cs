namespace TimezoneCli.Domain;

public sealed record WorkingHoursAssessment(
    bool IsWithinWorkingHours,
    string Reason);
