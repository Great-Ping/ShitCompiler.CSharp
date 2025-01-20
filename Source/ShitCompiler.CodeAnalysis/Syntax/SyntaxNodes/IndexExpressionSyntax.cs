using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record class IndexExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Identifier,
    Lexicon.Lexeme OpenParenthesisToken,
    SeparatedSyntaxList<ExpressionSyntax> Arguments,
    Lexicon.Lexeme CloseParenthesisToken
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