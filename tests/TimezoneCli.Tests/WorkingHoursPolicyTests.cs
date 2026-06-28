using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed class WorkingHoursPolicyTests
{
    [Fact]
    public void Assess_treats_weekday_0900_as_within_working_hours()
    {
        var assessment = new WorkingHoursPolicy().Assess(PlaceAt(new DateOnly(2026, 6, 29), new TimeOnly(9, 0)));

        Assert.True(assessment.IsWithinWorkingHours);
        Assert.Equal("weekday-within-hours", assessment.Reason);
    }

    [Fact]
    public void Assess_treats_weekday_1700_as_outside_working_hours()
    {
        var assessment = new WorkingHoursPolicy().Assess(PlaceAt(new DateOnly(2026, 6, 29), new TimeOnly(17, 0)));

        Assert.False(assessment.IsWithinWorkingHours);
        Assert.Equal("weekday-after-hours", assessment.Reason);
    }

    [Fact]
    public void Assess_treats_after_hours_as_outside()
    {
        var assessment = new WorkingHoursPolicy().Assess(PlaceAt(new DateOnly(2026, 6, 29), new TimeOnly(8, 59)));

        Assert.False(assessment.IsWithinWorkingHours);
    }

    [Fact]
    public void Assess_treats_weekend_as_outside()
    {
        var assessment = new WorkingHoursPolicy().Assess(PlaceAt(new DateOnly(2026, 6, 27), new TimeOnly(10, 0)));

        Assert.False(assessment.IsWithinWorkingHours);
        Assert.Equal("weekend", assessment.Reason);
    }

    [Fact]
    public void Assess_uses_custom_window_inclusive_start()
    {
        var window = new WorkingHoursWindow(new TimeOnly(8, 30), new TimeOnly(16, 45));

        var assessment = new WorkingHoursPolicy().Assess(
            PlaceAt(new DateOnly(2026, 6, 29), new TimeOnly(8, 30)),
            window);

        Assert.True(assessment.IsWithinWorkingHours);
        Assert.Equal("weekday-within-hours", assessment.Reason);
    }

    [Fact]
    public void Assess_uses_custom_window_exclusive_end()
    {
        var window = new WorkingHoursWindow(new TimeOnly(8, 30), new TimeOnly(16, 45));

        var assessment = new WorkingHoursPolicy().Assess(
            PlaceAt(new DateOnly(2026, 6, 29), new TimeOnly(16, 45)),
            window);

        Assert.False(assessment.IsWithinWorkingHours);
        Assert.Equal("weekday-after-hours", assessment.Reason);
    }

    private static ResolvedPlace PlaceAt(DateOnly date, TimeOnly time) =>
        new(
            "Test",
            "America/Mexico_City",
            date,
            time,
            TimeSpan.FromHours(-6),
            ResolvedPlaceSource.TimezoneIdentifier);
}
