
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis;
using ShitCompiler.CodeAnalysis.Syntax.Errors;

public class SemanticError(
    Location location,
    string? msg
) : ParseError(
    location,
    msg
);