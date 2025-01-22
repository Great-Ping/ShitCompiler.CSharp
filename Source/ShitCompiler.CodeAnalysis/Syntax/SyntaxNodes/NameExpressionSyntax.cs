using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record NameExpressionSyntax(
    Lexeme Identifier
) : ExpressionSyntax(SyntaxKind.NameExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return
        [
            Identifier
        ];
    }
};