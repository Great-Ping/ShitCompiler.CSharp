using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Lexicon.Errors;
using ShitCompiler.CodeAnalysis.Syntax.Errors;

namespace ShitCompiler.CodeAnalysis.Errors;

public class BadCharactersSquenceError(
    Location location,
    string sequence,
    string? message = null,
    Lexeme? badToken = null
) : LexiconError(location, message?? $"Bad sequence: \"{sequence}\"", badToken)
{
    public string Sequence => sequence;
};