namespace ShitCompiler.Lexicon.Exceptions;

public class IllegalSymbolException(
    string? message = null,
    Exception? innerException = null
) : Exception(message, innerException);