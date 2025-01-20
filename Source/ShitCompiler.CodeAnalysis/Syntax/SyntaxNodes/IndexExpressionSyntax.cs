using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record class IndexExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Identifier,
    Lexeme OpenParenthesisToken,
    SeparatedSyntaxList<ExpressionSyntax> Arguments,
    Lexeme CloseParenthesisToken
) : ExpressionSyntax(SymbolBlock, SyntaxKind.IndexExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return Enumerable.Concat(
            [Identifier, OpenParenthesisToken],
            Arguments.GetWithSeparators()
        ).Concat([CloseParenthesisToken]);
    }
}