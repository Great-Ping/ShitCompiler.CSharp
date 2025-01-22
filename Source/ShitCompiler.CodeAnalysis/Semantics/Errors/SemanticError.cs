using ShitCompiler.CodeAnalysis.Syntax.Errors;

namespace ShitCompiler.CodeAnalysis.Semantics.Errors;

public class SemanticError(
    Location location,
    string? msg
) : ParseError(
    location,
    msg
);