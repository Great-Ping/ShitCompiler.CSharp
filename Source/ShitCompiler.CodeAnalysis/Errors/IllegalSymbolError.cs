namespace ShitCompiler.CodeAnalysis.Lexicon.Exceptions;

public class IllegalSymbolError(
    string? message = null,
    Exception? innerException = null
) : Exception(message, innerException);