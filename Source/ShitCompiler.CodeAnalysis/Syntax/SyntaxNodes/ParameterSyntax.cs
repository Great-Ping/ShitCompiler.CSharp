using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ParameterSyntax(
    Lexeme Identifier,
    TypeClauseSyntax TypeClause
): MemberSyntax(
    SyntaxKind.ColonToken
) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return 
        [
            Identifier,
            TypeClause
        ];
    }

};