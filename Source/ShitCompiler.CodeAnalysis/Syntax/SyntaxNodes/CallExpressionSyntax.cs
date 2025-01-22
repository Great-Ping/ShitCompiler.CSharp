using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record CallExpressionSyntax(
    Lexeme Identifier,
    Lexeme OpenParenthesisToken,
    SeparatedSyntaxList<ExpressionSyntax> Arguments,
    Lexeme CloseParenthesisToken
) : ExpressionSyntax(SyntaxKind.CallExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return Enumerable
            .Concat<ISyntaxNode>(
                [
                    Identifier,
                    OpenParenthesisToken
                ],
                Arguments.GetWithSeparators()
            ).Concat(
                [CloseParenthesisToken]
            );
    }
}