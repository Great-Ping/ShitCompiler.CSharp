using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.Errors;

class UniquenessSymbolError(
    Location location, 
    string? message
) : ParseError(location, message)
{
    public UniquenessSymbolError(Lexeme identifier) 
        : this(identifier.Start, $"Not unique identifier {identifier.OriginalValue}")
    { }
}