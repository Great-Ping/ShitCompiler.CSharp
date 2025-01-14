using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Lexicon;

public record Lexeme(
    SyntaxKind Kind,
    string OriginalValue,
    Location Start
) : SyntaxNode(Kind);

public record Lexeme<T>(
    SyntaxKind Kind, 
    string OriginalValue,
    Location Start,
    T ParsedValue
): Lexeme(Kind, OriginalValue, Start);

public record BadLexeme(
    LexemeErrorCode ErrorCode,
    string OriginalValue,
    Location Start
): Lexeme(SyntaxKind.BadToken, OriginalValue, Start);

public enum LexemeErrorCode
{
    NotSupportedToken,
    BadCharacterSequence,
    IncompleteToken,
    TooManyCharactersInConstant
}