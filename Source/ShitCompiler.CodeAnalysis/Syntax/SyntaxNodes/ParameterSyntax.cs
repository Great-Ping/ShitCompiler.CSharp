using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ParameterSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Identifier,
    TypeClauseSyntax Type
): MemberSyntax(
    SymbolBlock,
    SyntaxKind.ColonToken
) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            Identifier,
            Type
        ];
    }

};