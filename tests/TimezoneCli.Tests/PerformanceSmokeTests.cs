using System.Diagnostics;
using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed class PerformanceSmokeTests
{
    [Fact]
    public void Lookup_completes_under_ten_seconds()
    {
        var service = CreateService();

        var elapsed = Measure(() => service.Lookup("America/Mexico_City"));

        Assert.True(elapsed < TimeSpan.FromSeconds(10), $"Lookup took {elapsed}.");
    }

    [Fact]
    public void Comparison_completes_under_fifteen_seconds()
    {
        var service = CreateService();

        var elapsed = Measure(() => service.Compare("America/Mexico_City", "Europe/London"));

        Assert.True(elapsed < TimeSpan.FromSeconds(15), $"Comparison took {elapsed}.");
    }

    [Fact]
    public void Custom_working_hours_comparison_completes_under_fifteen_seconds()
    {
        var service = CreateService();
        var window = new WorkingHoursWindow(new TimeOnly(8, 30), new TimeOnly(16, 45));

        var elapsed = Measure(() => service.Compare("America/Mexico_City", "Europe/London", window));

        Assert.True(elapsed < TimeSpan.FromSeconds(15), $"Custom comparison took {elapsed}.");
    }

    private static TimeSpan Measure(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    private static TimeComparisonService CreateService()
    {
        var timezoneResolver = new TimezoneResolver();
        var aliasResolver = new AliasResolver(PlaceAliasCatalog.LoadDefault(), timezoneResolver);
        return new TimeComparisonService(
            aliasResolver,
            new FixedTimeProvider(new DateTimeOffset(2026, 6, 27, 15, 30, 0, TimeSpan.Zero)),
            new WorkingHoursPolicy());
    }
}
