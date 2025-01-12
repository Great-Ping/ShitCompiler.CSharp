using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Lexicon.Errors;
using ShitCompiler.CodeAnalysis.Syntax.Errors;

namespace ShitCompiler.CodeAnalysis.Errors;

public class TooManyCharactersInConstant(
    Location location,
    string? message = null,
    Lexeme? badToken = null
) : LexiconError(location, message, badToken)
{
};