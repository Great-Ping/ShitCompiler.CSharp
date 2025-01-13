namespace ShitCompiler.CodeAnalysis.Lexicon.Errors;

public class TooManyCharactersInConstant(
    Location location,
    string? message = null,
    Lexeme? badToken = null
) : LexiconError(location, message, badToken)
{
};