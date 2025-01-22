using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ExpressionStatementSyntax(
    ExpressionSyntax Expression,
    Lexeme Semicolon
): StatementSyntax(
    SyntaxKind.ExpressionStatement
) {
    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            Expression,
            Semicolon
        ];
    }
}