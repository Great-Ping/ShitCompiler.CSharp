using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record LiteralExpressionSyntax<T>(
    Lexeme Token,
    T Value
) : LiteralExpressionSyntax(Token);

public abstract record LiteralExpressionSyntax(
    Lexeme Token
) : ExpressionSyntax(SyntaxKind.LiteralExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [Token];
    }
}
