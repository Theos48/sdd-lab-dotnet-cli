namespace TimezoneCli.Domain;

public sealed class WorkingHoursPolicy
{
    private static readonly TimeOnly Start = new(9, 0);
    private static readonly TimeOnly End = new(17, 0);

    public WorkingHoursAssessment Assess(ResolvedPlace place)
    {
        var dayOfWeek = place.LocalDate.DayOfWeek;
        if (dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return new WorkingHoursAssessment(false, "weekend");
        }

        if (place.LocalTime >= Start && place.LocalTime < End)
        {
            return new WorkingHoursAssessment(true, "weekday-within-hours");
        }

        return new WorkingHoursAssessment(false, "weekday-after-hours");
    }
}
