using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ExpressionStatementSyntax(
    SymbolBlock SymbolBlock,
    ExpressionSyntax Expression,
    Lexeme Semicolon
): StatementSyntax(
    SymbolBlock,
    SyntaxKind.ExpressionStatement
) {
    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            Expression,
            Semicolon
        ];
    }
}