
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record VariableDeclarationSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Keyword,
    Lexeme Identifier,
    TypeClauseSyntax? TypeClause,
    Lexeme EqualsToken,
    ExpressionSyntax initializer
): StatementSyntax(
    SymbolBlock,
    SyntaxKind.VariableDeclaration
) {

    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            Keyword,
            Identifier,
            TypeClause,
            EqualsToken,
            initializer
        ];
    }

}