using TimezoneCli.Domain;

namespace TimezoneCli.Tests;

public sealed class WorkingHoursWindowTests
{
    [Fact]
    public void Default_uses_0900_to_1700()
    {
        Assert.Equal(new TimeOnly(9, 0), WorkingHoursWindow.Default.Start);
        Assert.Equal(new TimeOnly(17, 0), WorkingHoursWindow.Default.End);
        Assert.Equal("09:00-17:00", WorkingHoursWindow.Default.Format());
    }

    [Fact]
    public void Parse_accepts_strict_hhmm_values()
    {
        var result = WorkingHoursWindow.Parse("08:30", "16:45");

        Assert.True(result.IsSuccess);
        Assert.Equal(new TimeOnly(8, 30), result.Value!.Start);
        Assert.Equal(new TimeOnly(16, 45), result.Value.End);
        Assert.Equal("08:30-16:45", result.Value.Format());
    }

    [Theory]
    [InlineData("9am")]
    [InlineData("9:00")]
    [InlineData("0900")]
    [InlineData("24:00")]
    [InlineData("17:60")]
    public void Parse_rejects_non_strict_or_invalid_start_values(string start)
    {
        var result = WorkingHoursWindow.Parse(start, "17:00");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.InvalidWorkingHoursTime, result.Error!.Kind);
        Assert.Equal(start, result.Error.Input);
    }

    [Theory]
    [InlineData("17:00", "17:00")]
    [InlineData("17:00", "09:00")]
    public void Parse_rejects_zero_length_and_overnight_ranges(string start, string end)
    {
        var result = WorkingHoursWindow.Parse(start, end);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResolutionErrorKind.InvalidWorkingHoursRange, result.Error!.Kind);
    }

    [Theory]
    [InlineData(17, 0, 17, 0)]
    [InlineData(17, 0, 9, 0)]
    public void Constructor_rejects_zero_length_and_overnight_ranges(
        int startHour,
        int startMinute,
        int endHour,
        int endMinute)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new WorkingHoursWindow(
                new TimeOnly(startHour, startMinute),
                new TimeOnly(endHour, endMinute)));

        Assert.Equal("end", exception.ParamName);
    }
}
