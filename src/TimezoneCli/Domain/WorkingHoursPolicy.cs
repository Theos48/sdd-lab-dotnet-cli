namespace TimezoneCli.Domain;

public sealed class WorkingHoursPolicy
{
    public WorkingHoursAssessment Assess(ResolvedPlace place) =>
        Assess(place, WorkingHoursWindow.Default);

    public WorkingHoursAssessment Assess(ResolvedPlace place, WorkingHoursWindow window)
    {
        var dayOfWeek = place.LocalDate.DayOfWeek;
        if (dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return new WorkingHoursAssessment(false, "weekend");
        }

        if (place.LocalTime >= window.Start && place.LocalTime < window.End)
        {
            return new WorkingHoursAssessment(true, "weekday-within-hours");
        }

        return new WorkingHoursAssessment(false, "weekday-after-hours");
    }
}
