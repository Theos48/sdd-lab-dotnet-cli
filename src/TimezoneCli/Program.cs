using TimezoneCli.Cli;
using TimezoneCli.Domain;

var parseResult = CliParser.Parse(args);
if (!parseResult.IsSuccess)
{
    return Emit(CliResultWriter.Error(parseResult.Error!));
}

try
{
    var timezoneResolver = new TimezoneResolver();
    var catalog = PlaceAliasCatalog.LoadDefault();
    var aliasResolver = new AliasResolver(catalog, timezoneResolver);
    var service = new TimeComparisonService(aliasResolver, TimeProvider.System, new WorkingHoursPolicy());
    var options = parseResult.Value!;

    if (options.Compare is null)
    {
        var lookup = service.Lookup(options.Place);
        return lookup.IsSuccess
            ? Emit(CliResultWriter.Lookup(lookup.Value!))
            : Emit(CliResultWriter.Error(lookup.Error!));
    }

    var comparison = service.Compare(options.Place, options.Compare, options.WorkingHoursWindow);
    return comparison.IsSuccess
        ? Emit(CliResultWriter.Comparison(comparison.Value!))
        : Emit(CliResultWriter.Error(comparison.Error!));
}
catch (InvalidOperationException exception)
{
    Console.Error.WriteLine($"Error: {exception.Message}");
    return ExitCodes.InvalidInput;
}

static int Emit(CliResult result)
{
    foreach (var line in result.StdoutLines)
    {
        Console.Out.WriteLine(line);
    }

    foreach (var line in result.StderrLines)
    {
        Console.Error.WriteLine(line);
    }

    return result.ExitCode;
}
