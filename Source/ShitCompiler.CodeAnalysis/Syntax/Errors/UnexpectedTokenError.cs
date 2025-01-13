using ShitCompiler.CodeAnalysis.Errors;

namespace ShitCompiler.CodeAnalysis.Syntax.Errors;

public class UnexpectedTokenError(
    Location location,
    string? message
) : ParseError(location, message);