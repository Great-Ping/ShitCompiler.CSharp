namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record TypeClauseSyntax(
    SymbolBlock SymbolBlock,
    Lexeme ColonToken,
    Lexeme Identifier
) : SyntaxNode(SyntaxKind.TypeClause) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            ColonToken,
            Identifier
        ];
    }

}