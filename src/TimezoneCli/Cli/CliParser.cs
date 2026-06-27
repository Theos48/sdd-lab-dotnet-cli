using TimezoneCli.Domain;

namespace TimezoneCli.Cli;

public static class CliParser
{
    public static ResolutionResult<CliOptions> Parse(string[] args)
    {
        string? place = null;
        string? compare = null;
        var compareCount = 0;

        for (var i = 0; i < args.Length; i++)
        {
            var argument = args[i];
            if (argument is "--place")
            {
                if (!TryReadValue(args, ref i, out place))
                {
                    return ResolutionResult<CliOptions>.Failure(
                        ResolutionError.InvalidArgument("--place"));
                }

                continue;
            }

            if (argument.StartsWith("--place=", StringComparison.Ordinal))
            {
                place = argument["--place=".Length..];
                continue;
            }

            if (argument is "--compare")
            {
                compareCount++;
                if (compareCount > 1)
                {
                    return ResolutionResult<CliOptions>.Failure(
                        ResolutionError.TooManyComparisonPlaces());
                }

                if (!TryReadValue(args, ref i, out compare))
                {
                    return ResolutionResult<CliOptions>.Failure(
                        ResolutionError.InvalidArgument("--compare"));
                }

                continue;
            }

            if (argument.StartsWith("--compare=", StringComparison.Ordinal))
            {
                compareCount++;
                if (compareCount > 1)
                {
                    return ResolutionResult<CliOptions>.Failure(
                        ResolutionError.TooManyComparisonPlaces());
                }

                compare = argument["--compare=".Length..];
                continue;
            }

            if (compare is not null && !argument.StartsWith("--", StringComparison.Ordinal))
            {
                return ResolutionResult<CliOptions>.Failure(
                    ResolutionError.TooManyComparisonPlaces());
            }

            return ResolutionResult<CliOptions>.Failure(
                ResolutionError.InvalidArgument(argument));
        }

        if (string.IsNullOrWhiteSpace(place))
        {
            return ResolutionResult<CliOptions>.Failure(ResolutionError.MissingPlace());
        }

        return ResolutionResult<CliOptions>.Success(new CliOptions(place, compare));
    }

    private static bool TryReadValue(string[] args, ref int index, out string? value)
    {
        value = null;
        if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
        {
            return false;
        }

        index++;
        value = args[index];
        return true;
    }
}
