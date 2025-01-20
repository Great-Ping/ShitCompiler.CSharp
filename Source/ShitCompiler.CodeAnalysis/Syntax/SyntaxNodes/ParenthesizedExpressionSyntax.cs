using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ParenthesizedExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Left,
    ExpressionSyntax Expression,
    Lexicon.Lexeme Right
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