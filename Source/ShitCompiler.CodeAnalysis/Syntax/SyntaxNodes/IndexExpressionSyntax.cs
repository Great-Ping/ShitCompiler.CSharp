using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record class IndexExpressionSyntax(
    Lexeme Identifier,
    Lexeme OpenParenthesisToken,
    LiteralExpressionSyntax<long> Number,
    Lexeme CloseParenthesisToken
) : ExpressionSyntax(SyntaxKind.IndexExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return OpenParenthesisToken;
        yield return Number;
        yield return CloseParenthesisToken;
    }
}