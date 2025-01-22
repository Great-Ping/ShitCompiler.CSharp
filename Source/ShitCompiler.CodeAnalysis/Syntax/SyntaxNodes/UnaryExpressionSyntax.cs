using System.Diagnostics.SymbolStore;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record UnaryExpressionSyntax(
    Lexeme Operator,
    ExpressionSyntax Operand
) : ExpressionSyntax(SyntaxKind.UnaryExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [
            Operator, 
            Operand
        ];
    }
};