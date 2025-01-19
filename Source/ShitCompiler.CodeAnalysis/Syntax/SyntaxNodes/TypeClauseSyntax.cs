namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;

public sealed record IdentifierTypeSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Identifier
) : TypeSyntax(SymbolBlock, SyntaxKind.IdentifierTypeSyntax)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return [Identifier];
    }
}


public sealed record ArrayTypeSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Identifier,
    Lexeme OpenBracket,
    Lexeme<long> ArraySizeNumber,
    Lexeme CloseBracket
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