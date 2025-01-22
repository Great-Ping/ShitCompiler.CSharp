using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record ArrayExpressionSyntax(
    SyntaxKind Kind,
    Lexeme OpenBrace,
    SeparatedSyntaxList<ExpressionSyntax> Expressions,
    Lexeme CloseBrace
) : ExpressionSyntax(Kind)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return Enumerable.Concat<ISyntaxNode>(
            [OpenBrace],
            Expressions.GetWithSeparators()
        ).Concat([CloseBrace]);
    }
}