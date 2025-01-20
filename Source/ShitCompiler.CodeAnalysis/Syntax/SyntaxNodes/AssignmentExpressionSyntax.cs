using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;


public sealed record ArrayAssigmentExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Identifier,
    Lexicon.Lexeme OpenBracket,
    ExpressionSyntax Expression,
    Lexicon.Lexeme CloseBracket,
    Lexicon.Lexeme Operator,
    ExpressionSyntax Right
) : ExpressionSyntax(SymbolBlock, SyntaxKind.ArrayAssigmentExpression)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return OpenBracket;
        yield return Expression;
        yield return CloseBracket;
        yield return Operator;
        yield return Right;
    }
}

public sealed record AssignmentExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Identifier,
    Lexicon.Lexeme Operator,
    ExpressionSyntax Right
) : ExpressionSyntax(SymbolBlock, SyntaxKind.AssignmentExpression)
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