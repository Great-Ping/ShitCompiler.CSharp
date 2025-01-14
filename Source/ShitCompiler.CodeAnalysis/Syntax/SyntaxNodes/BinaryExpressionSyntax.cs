using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record BinaryExpressionSyntax(
    SymbolBlock SymbolBlock, 
    ExpressionSyntax Left,
    Lexeme Operand,
    ExpressionSyntax right
) : ExpressionSyntax(SymbolBlock, SyntaxKind.BinaryExpression) {
    
}