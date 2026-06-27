namespace TimezoneCli.Cli;

public sealed record CliResult(
    int ExitCode,
    IReadOnlyList<string> StdoutLines,
    IReadOnlyList<string> StderrLines);
