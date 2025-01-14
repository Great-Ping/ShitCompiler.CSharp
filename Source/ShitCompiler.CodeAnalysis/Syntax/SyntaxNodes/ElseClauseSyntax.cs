
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using ShitCompiler.CodeAnalysis.Lexicon;

using System.Linq;

public record ElseClauseSyntax(
    SymbolBlock SymbolBlock,
    Lexeme elseKeyword,
    StatementSyntax elseStatement
): MemberSyntax(
    SymbolBlock,
    SyntaxKind.ElseKeyword
) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            elseKeyword,
            elseStatement
        ];
    }

}