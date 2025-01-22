using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ElseClauseSyntax(
    Lexeme ElseKeyword,
    StatementSyntax ElseStatement
): MemberSyntax(
    SyntaxKind.ElseKeyword
) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            ElseKeyword,
            ElseStatement
        ];
    }

}