namespace TimezoneCli.Domain;

public sealed class AliasCatalog
{
    public string Version { get; init; } = string.Empty;

    public List<SupportedAlias> Aliases { get; init; } = [];
}
