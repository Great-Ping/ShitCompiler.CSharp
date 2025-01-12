using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Lexicon;

public record Lexeme(
    SyntaxKind Kind, 
    string OriginalValue,
    Location Start
): ISyntaxNode;

public record Lexeme<T>(
    SyntaxKind Kind, 
    string OriginalValue,
    Location Start,
    T ParsedValue
): Lexeme(Kind, OriginalValue, Start);