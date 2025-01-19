using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record BinaryExpressionSyntax(
    SymbolBlock SymbolBlock, 
    ExpressionSyntax Left,
    Lexeme Operand,
    ExpressionSyntax Right
) : ExpressionSyntax(SymbolBlock, SyntaxKind.BinaryExpression) {
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return
        [
            Left,
            Operand,
            Right
        ];
    }
}