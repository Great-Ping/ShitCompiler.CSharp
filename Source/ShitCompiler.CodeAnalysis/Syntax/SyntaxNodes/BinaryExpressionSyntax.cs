using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record BinaryExpressionSyntax(
    ExpressionSyntax Left,
    Lexeme Operand,
    ExpressionSyntax Right
) : ExpressionSyntax(SyntaxKind.BinaryExpression) {
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [
            Left,
            Operand,
            Right
        ];
    }
}