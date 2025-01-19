using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record NameExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Identifier
) : ExpressionSyntax(SymbolBlock, SyntaxKind.NameExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return
        [
            Identifier
        ];
    }
};