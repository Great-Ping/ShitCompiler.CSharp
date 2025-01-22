using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ParenthesizedExpressionSyntax(
    Lexeme Left,
    ExpressionSyntax Expression,
    Lexeme Right
) : ExpressionSyntax(SyntaxKind.ParenthesizedExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return
        [
            Left, 
            Expression, 
            Right
        ];
    }
};