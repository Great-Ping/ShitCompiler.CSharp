using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Lexicon.Errors;

namespace ShitCompiler.CodeAnalysis.Errors;

public class IncompleteTokenError(
    Location location, 
    string? message = null, 
    Lexeme? badToken = null
) : LexiconError(location, message, badToken){
    
}