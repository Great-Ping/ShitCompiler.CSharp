using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ElseClauseSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme ElseKeyword,
    StatementSyntax ElseStatement
): MemberSyntax(
    SymbolBlock,
    SyntaxKind.ElseKeyword
) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            ElseKeyword,
            ElseStatement
        ];
    }

}