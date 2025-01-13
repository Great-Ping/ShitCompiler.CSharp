namespace ShitCompiler.CodeAnalysis.Lexicon.Errors;

public class BadCharactersSquenceError(
    Location location,
    string sequence,
    string? message = null,
    Lexeme? badToken = null
) : LexiconError(location, message?? $"Bad sequence: \"{sequence}\"", badToken)
{
    public string Sequence => sequence;
};