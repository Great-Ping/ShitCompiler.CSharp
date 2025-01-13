namespace ShitCompiler.CodeAnalysis.Errors;

public class IllegalSymbolError(
    string? message = null,
    Exception? innerException = null
) : Exception(message, innerException);