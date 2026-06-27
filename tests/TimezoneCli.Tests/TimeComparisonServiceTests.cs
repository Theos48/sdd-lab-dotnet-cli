using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed class TimeComparisonServiceTests
{
    [Fact]
    public void Lookup_uses_current_moment_local_date_time_and_offset()
    {
        var service = CreateService(new DateTimeOffset(2026, 6, 27, 15, 30, 0, TimeSpan.Zero));

        var result = service.Lookup("America/Mexico_City");

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(2026, 6, 27), result.Value!.LocalDate);
        Assert.Equal(new TimeOnly(9, 30), result.Value.LocalTime);
        Assert.Equal(TimeSpan.FromHours(-6), result.Value.UtcOffset);
    }

    [Fact]
    public void Compare_same_timezone_reports_zero_difference()
    {
        var service = CreateService(new DateTimeOffset(2026, 6, 27, 15, 30, 0, TimeSpan.Zero));

        var result = service.Compare("America/Mexico_City", "mexico city");

        Assert.True(result.IsSuccess);
        Assert.Equal(TimeSpan.Zero, result.Value!.SignedTimeDifference);
    }

    [Fact]
    public void Compare_different_timezones_reports_signed_difference()
    {
        var service = CreateService(new DateTimeOffset(2026, 1, 15, 15, 0, 0, TimeSpan.Zero));

        var result = service.Compare("America/Mexico_City", "Europe/London");

        Assert.True(result.IsSuccess);
        Assert.Equal(TimeSpan.FromHours(6), result.Value!.SignedTimeDifference);
    }

    [Fact]
    public void Compare_preserves_different_local_dates()
    {
        var service = CreateService(new DateTimeOffset(2026, 1, 1, 1, 30, 0, TimeSpan.Zero));

        var result = service.Compare("America/Mexico_City", "Asia/Tokyo");

        Assert.True(result.IsSuccess);
        Assert.NotEqual(result.Value!.RequestedPlace.LocalDate, result.Value.ComparisonPlace.LocalDate);
    }

    [Fact]
    public void Compare_is_suitable_only_when_both_places_are_within_working_hours()
    {
        var service = CreateService(new DateTimeOffset(2026, 1, 15, 15, 0, 0, TimeSpan.Zero));

        var result = service.Compare("America/Mexico_City", "Europe/London");

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.RequestedWorkingHours.IsWithinWorkingHours);
        Assert.False(result.Value.ComparisonWorkingHours.IsWithinWorkingHours);
        Assert.False(result.Value.CombinedSuitability);
    }

    private static TimeComparisonService CreateService(DateTimeOffset utcNow)
    {
        var catalog = PlaceAliasCatalog.LoadDefault();
        var timezoneResolver = new TimezoneResolver();
        var aliasResolver = new AliasResolver(catalog, timezoneResolver);
        return new TimeComparisonService(aliasResolver, new FixedTimeProvider(utcNow), new WorkingHoursPolicy());
    }
}
