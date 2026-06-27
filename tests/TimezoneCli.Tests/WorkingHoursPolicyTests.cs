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

    private static ResolvedPlace PlaceAt(DateOnly date, TimeOnly time) =>
        new(
            "Test",
            "America/Mexico_City",
            date,
            time,
            TimeSpan.FromHours(-6),
            ResolvedPlaceSource.TimezoneIdentifier);
}
