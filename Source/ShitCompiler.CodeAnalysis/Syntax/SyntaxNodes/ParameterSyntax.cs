using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

using System.Linq;
using ShitCompiler.CodeAnalysis.Syntax;

public record ParameterSyntax(
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