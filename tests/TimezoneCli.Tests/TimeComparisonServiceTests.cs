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
    public void Compare_resolves_supported_aliases_on_both_sides()
    {
        var service = CreateService(new DateTimeOffset(2026, 1, 15, 15, 0, 0, TimeSpan.Zero));

        var result = service.Compare("mexico city", "london");

        Assert.True(result.IsSuccess);
        Assert.Equal("Mexico City", result.Value!.RequestedPlace.DisplayName);
        Assert.Equal("America/Mexico_City", result.Value.RequestedPlace.TimeZoneId);
        Assert.Equal("London", result.Value.ComparisonPlace.DisplayName);
        Assert.Equal("Europe/London", result.Value.ComparisonPlace.TimeZoneId);
        Assert.Equal(TimeSpan.FromHours(6), result.Value.SignedTimeDifference);
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
    public void Compare_returns_unknown_when_comparison_place_is_unknown()
    {
        var service = CreateService(new DateTimeOffset(2026, 1, 15, 15, 0, 0, TimeSpan.Zero));

        var result = service.Compare("mexico city", "Atlantis");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.UnknownPlace, result.Error!.Kind);
        Assert.Equal("Atlantis", result.Error.Input);
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
        var service = CreateService(new DateTimeOffset(2026, 1, 15, 17, 0, 0, TimeSpan.Zero));

        var result = service.Compare("America/Mexico_City", "Europe/London");

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.RequestedWorkingHours.IsWithinWorkingHours);
        Assert.False(result.Value.ComparisonWorkingHours.IsWithinWorkingHours);
        Assert.False(result.Value.CombinedSuitability);
        Assert.Equal(WorkingHoursWindow.Default, result.Value.WorkingHoursWindow);
    }

    [Fact]
    public void Compare_uses_custom_working_hours_for_both_places()
    {
        var service = CreateService(new DateTimeOffset(2026, 1, 15, 14, 0, 0, TimeSpan.Zero));
        var window = new WorkingHoursWindow(new TimeOnly(8, 0), new TimeOnly(15, 0));

        var result = service.Compare("America/Mexico_City", "Europe/London", window);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.RequestedWorkingHours.IsWithinWorkingHours);
        Assert.True(result.Value.ComparisonWorkingHours.IsWithinWorkingHours);
        Assert.True(result.Value.CombinedSuitability);
        Assert.Equal(window, result.Value.WorkingHoursWindow);
    }

    [Fact]
    public void Compare_preserves_behavior_for_supported_postal_code_with_custom_working_hours()
    {
        var service = CreateService(new DateTimeOffset(2026, 1, 15, 14, 30, 0, TimeSpan.Zero));
        var window = new WorkingHoursWindow(new TimeOnly(8, 30), new TimeOnly(16, 45));

        var result = service.Compare("01000", "london", window);

        Assert.True(result.IsSuccess);
        Assert.Equal("01000", result.Value!.RequestedPlace.DisplayName);
        Assert.Equal("America/Mexico_City", result.Value.RequestedPlace.TimeZoneId);
        Assert.Equal(ResolvedPlaceSource.SupportedMexicanPostalCode, result.Value.RequestedPlace.Source);
        Assert.Equal("London", result.Value.ComparisonPlace.DisplayName);
        Assert.Equal(TimeSpan.FromHours(6), result.Value.SignedTimeDifference);
        Assert.True(result.Value.RequestedWorkingHours.IsWithinWorkingHours);
        Assert.True(result.Value.ComparisonWorkingHours.IsWithinWorkingHours);
        Assert.True(result.Value.CombinedSuitability);
        Assert.Equal(window, result.Value.WorkingHoursWindow);
    }

    [Fact]
    public void Compare_marks_not_suitable_when_one_place_is_outside_custom_window()
    {
        var service = CreateService(new DateTimeOffset(2026, 1, 15, 15, 0, 0, TimeSpan.Zero));
        var window = new WorkingHoursWindow(new TimeOnly(8, 0), new TimeOnly(10, 0));

        var result = service.Compare("America/Mexico_City", "Europe/London", window);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.RequestedWorkingHours.IsWithinWorkingHours);
        Assert.False(result.Value.ComparisonWorkingHours.IsWithinWorkingHours);
        Assert.False(result.Value.CombinedSuitability);
        Assert.Equal(window, result.Value.WorkingHoursWindow);
    }

    private static TimeComparisonService CreateService(DateTimeOffset utcNow)
    {
        var catalog = PlaceAliasCatalog.LoadDefault();
        var timezoneResolver = new TimezoneResolver();
        var aliasResolver = new AliasResolver(catalog, timezoneResolver);
        return new TimeComparisonService(aliasResolver, new FixedTimeProvider(utcNow), new WorkingHoursPolicy());
    }
}
