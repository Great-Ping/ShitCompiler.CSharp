namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ExpressionStatementSyntax(
    SymbolBlock SymbolBlock,
    ExpressionSyntax Expression
): StatementSyntax(
    SymbolBlock,
    SyntaxKind.ExpressionStatement
) {
    public override IEnumerable<ISyntaxNode> GetChildren() {
        throw new NotImplementedException(nameof(ExpressionStatementSyntax));
        return [
            // ExpressionSyntax
        ];
    }
}