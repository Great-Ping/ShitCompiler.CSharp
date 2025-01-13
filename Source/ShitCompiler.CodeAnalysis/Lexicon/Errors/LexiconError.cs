using ShitCompiler.CodeAnalysis.Errors;

namespace ShitCompiler.CodeAnalysis.Lexicon.Errors;

public class LexiconError(
    Location location,
    string? message = null,
    Lexeme? badToken = null
) : ParseError(location, message)
{
    public Lexeme? BadToken => badToken;

    public override string ToString()
    {
        return $"{this.GetType().Name}, {{ Message = {this.Message}, Location = {this.Location} BadToken = {BadToken} }}";
    }
};
