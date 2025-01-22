using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;


public sealed record ArrayAssigmentExpressionSyntax(
    Lexeme Identifier,
    Lexeme OpenBracket,
    LiteralExpressionSyntax<long> Number,
    Lexeme CloseBracket,
    Lexeme Operator,
    ExpressionSyntax Right
) : ExpressionSyntax(SyntaxKind.ArrayAssigmentExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return OpenBracket;
        yield return Number;
        yield return CloseBracket;
        yield return Operator;
        yield return Right;
    }
}

public sealed record AssignmentExpressionSyntax(
    Lexeme Identifier,
    Lexeme Operator,
    ExpressionSyntax Right
) : ExpressionSyntax(SyntaxKind.AssignmentExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [
            Identifier,
            Operator,
            Right
        ];
    }
};