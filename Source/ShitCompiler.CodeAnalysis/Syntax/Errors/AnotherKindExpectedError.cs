using ShitCompiler.CodeAnalysis.Errors;

namespace ShitCompiler.CodeAnalysis.Syntax.Errors;

public class AnotherTokenExpectedError(
    Location location,
    string? message
) : ParseError(location, message);