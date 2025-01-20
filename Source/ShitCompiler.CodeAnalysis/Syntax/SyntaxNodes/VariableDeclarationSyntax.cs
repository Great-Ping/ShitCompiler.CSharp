using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record VariableDeclarationSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Keyword,
    Lexicon.Lexeme Identifier,
    TypeClauseSyntax? TypeClause,
    Lexicon.Lexeme EqualsToken,
    ExpressionSyntax Initializer,
    Lexicon.Lexeme SemicolonToken
): StatementSyntax(
    SymbolBlock,
    SyntaxKind.VariableDeclaration
) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return new List<ISyntaxNode?>(){
            Keyword,
            Identifier,
            TypeClause,
            EqualsToken,
            Initializer,
            SemicolonToken
        }.Where(n => n is not null)!;
    }

}