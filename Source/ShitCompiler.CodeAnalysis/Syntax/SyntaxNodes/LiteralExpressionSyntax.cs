using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record LiteralExpressionSyntax<T>(
    SymbolBlock SymbolBlock,
    Lexeme Token,
    T Value
) : LiteralExpressionSyntax(SymbolBlock, Token);

public abstract record LiteralExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Token
) : ExpressionSyntax(SymbolBlock, SyntaxKind.LiteralExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [Token];
    }
}
