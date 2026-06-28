using System.Globalization;

namespace TimezoneCli.Domain;

public sealed record WorkingHoursWindow
{
    public WorkingHoursWindow(TimeOnly start, TimeOnly end)
    {
        if (end <= start)
        {
            throw new ArgumentException(
                "Working-hours end must be later than start.",
                nameof(end));
        }

        Start = start;
        End = end;
    }

    public TimeOnly Start { get; }

    public TimeOnly End { get; }

    public static WorkingHoursWindow Default { get; } =
        new(new TimeOnly(9, 0), new TimeOnly(17, 0));

    public string Format() =>
        $"{Start:HH\\:mm}-{End:HH\\:mm}";

    public static ResolutionResult<WorkingHoursWindow> Parse(string start, string end)
    {
        if (!TryParseTime(start, out var parsedStart))
        {
            return ResolutionResult<WorkingHoursWindow>.Failure(
                ResolutionError.InvalidWorkingHoursTime(start));
        }

        if (!TryParseTime(end, out var parsedEnd))
        {
            return ResolutionResult<WorkingHoursWindow>.Failure(
                ResolutionError.InvalidWorkingHoursTime(end));
        }

        if (parsedEnd <= parsedStart)
        {
            return ResolutionResult<WorkingHoursWindow>.Failure(
                ResolutionError.InvalidWorkingHoursRange());
        }

        return ResolutionResult<WorkingHoursWindow>.Success(new WorkingHoursWindow(parsedStart, parsedEnd));
    }

    private static bool TryParseTime(string input, out TimeOnly value) =>
        TimeOnly.TryParseExact(
            input,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out value);
}
