using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record class IndexExpressionSyntax(
    Lexeme Identifier,
    Lexeme OpenParenthesisToken,
    SeparatedSyntaxList<ExpressionSyntax> Arguments,
    Lexeme CloseParenthesisToken
) : ExpressionSyntax(SyntaxKind.IndexExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return Enumerable.Concat(
            [Identifier, OpenParenthesisToken],
            Arguments.GetWithSeparators()
        ).Concat([CloseParenthesisToken]);
    }
}