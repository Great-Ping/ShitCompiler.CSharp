using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record ArrayExpressionSyntax(
    SymbolBlock SymbolBlock,
    SyntaxKind Kind,
    Lexicon.Lexeme OpenBrace,
    SeparatedSyntaxList<ExpressionSyntax> Expressions,
    Lexicon.Lexeme CloseBrace
) : ExpressionSyntax(SymbolBlock, Kind)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return Enumerable.Concat<ISyntaxNode>(
            [OpenBrace],
            Expressions.GetWithSeparators()
        ).Concat([CloseBrace]);
    }
}