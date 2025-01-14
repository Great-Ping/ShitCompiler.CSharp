using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record VariableDeclarationSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Keyword,
    Lexeme Identifier,
    TypeClauseSyntax? TypeClause,
    Lexeme EqualsToken,
    ExpressionSyntax Initializer
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
            Initializer
        }.Where(n => n is not null)!;
    }

}