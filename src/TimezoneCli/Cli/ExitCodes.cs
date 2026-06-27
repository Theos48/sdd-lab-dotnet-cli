using TimezoneCli.Domain;

namespace TimezoneCli.Cli;

public static class ExitCodes
{
    public const int Success = 0;
    public const int InvalidInput = 1;
    public const int UnsupportedInput = 2;
    public const int UnknownInput = 3;
    public const int AmbiguousInput = 4;

    public static int FromErrorKind(ResolutionErrorKind kind) =>
        kind switch
        {
            ResolutionErrorKind.UnsupportedMexicanPostalCode => UnsupportedInput,
            ResolutionErrorKind.UnknownPlace => UnknownInput,
            ResolutionErrorKind.AmbiguousPlace => AmbiguousInput,
            _ => InvalidInput,
        };
}
