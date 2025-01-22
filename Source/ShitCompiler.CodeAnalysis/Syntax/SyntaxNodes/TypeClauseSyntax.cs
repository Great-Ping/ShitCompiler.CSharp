namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;

public sealed record IdentifierTypeSyntax(
    Lexeme Identifier
) : TypeSyntax(SyntaxKind.IdentifierTypeSyntax)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [Identifier];
    }
}


public sealed record ArrayTypeSyntax(
    Lexeme Identifier,
    Lexeme OpenBracket,
    LiteralExpressionSyntax<long> ArraySizeNumber,
    Lexeme CloseBracket
) : TypeSyntax(SyntaxKind.ArrayTypeSyntax)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return
        [
            Identifier,
            OpenBracket,
            ArraySizeNumber,
            CloseBracket
        ];
    }
}

public abstract record TypeSyntax(
    SyntaxKind Kind
) : SyntaxNode(Kind);


public sealed record TypeClauseSyntax(
    Lexeme ColonToken,
    TypeSyntax Type
) : SyntaxNode(SyntaxKind.TypeClause) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            ColonToken,
            Type
        ];
    }

}