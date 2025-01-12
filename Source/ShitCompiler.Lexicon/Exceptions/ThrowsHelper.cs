namespace ShitCompiler.Lexicon.Exceptions;

public static class ThrowsHelper
{

    public static void ThrowIllegalSymbol(Location location)
    {
        throw new IllegalSymbolException($"Unknown symbol detected {location}");
    }
}