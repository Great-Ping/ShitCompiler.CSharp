using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Semantics;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record LiteralExpressionSyntax<T>(
    Lexeme Token,
    DataType Type,
    T Value
) : LiteralExpressionSyntax(Token, Type);

public abstract record LiteralExpressionSyntax(
    Lexeme Token,
    DataType Type
) : ExpressionSyntax(SyntaxKind.LiteralExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [Token];
    }
}
