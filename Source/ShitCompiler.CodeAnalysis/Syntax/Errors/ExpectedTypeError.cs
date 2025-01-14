using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.Errors;

public class ExpectedTypeError(
    Location location, 
    string? message
) : ParseError(location, message)
{
    public ExpectedTypeError(Lexeme lexeme)
        : this(lexeme.Start, $"Expected Type, but found {lexeme.Kind}")
    { }
}