namespace ShitCompiler.CodeAnalysis.Lexicon.Errors;

public class IncompleteTokenError(
    Location location, 
    string? message = null, 
    Lexeme? badToken = null
) : LexiconError(location, message, badToken){
    
}