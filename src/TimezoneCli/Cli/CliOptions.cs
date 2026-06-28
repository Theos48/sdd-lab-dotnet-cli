using TimezoneCli.Domain;

namespace TimezoneCli.Cli;

public sealed record CliOptions(
    string Place,
    string? Compare,
    WorkingHoursWindow? WorkingHoursWindow = null);
