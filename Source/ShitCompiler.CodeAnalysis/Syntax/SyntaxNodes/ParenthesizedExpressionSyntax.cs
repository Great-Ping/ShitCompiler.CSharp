using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ParenthesizedExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Left,
    ExpressionSyntax Expression,
    Lexeme Right
) : ExpressionSyntax(SymbolBlock, SyntaxKind.ParenthesizedExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return
        [
            Left, 
            Expression, 
            Right
        ];
    }
};