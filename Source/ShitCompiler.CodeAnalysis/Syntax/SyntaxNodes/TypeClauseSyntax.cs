namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;

public sealed record IdentifierTypeSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Identifier
) : TypeSyntax(SymbolBlock, SyntaxKind.IdentifierTypeSyntax)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [Identifier];
    }
}


public sealed record ArrayTypeSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Identifier,
    Lexicon.Lexeme OpenBracket,
    ExpressionSyntax ArraySizeNumber,
    Lexicon.Lexeme CloseBracket
) : TypeSyntax(SymbolBlock, SyntaxKind.ArrayTypeSyntax)
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
    SymbolBlock SymbolBlock,
    SyntaxKind Kind
) : SyntaxNode(Kind);


public sealed record TypeClauseSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme ColonToken,
    TypeSyntax Type
) : SyntaxNode(SyntaxKind.TypeClause) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            ColonToken,
            Type
        ];
    }

}