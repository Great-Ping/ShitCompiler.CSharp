using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record AssignmentExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Identifier,
    Lexeme Operator,
    ExpressionSyntax Left
) : ExpressionSyntax(SymbolBlock, SyntaxKind.AssignmentExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [
            Identifier,
            Operator,
            Left
        ];
    }
};