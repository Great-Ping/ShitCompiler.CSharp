using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record ParenthesizedExpressionSyntax(
    SymbolBlock SymbolBlock, 
    Lexeme Left, 
    ExpressionSyntax Expression, 
    Lexeme Right
) : ExpressionSyntax(SymbolBlock, SyntaxKind.ParenthesizedExpression);