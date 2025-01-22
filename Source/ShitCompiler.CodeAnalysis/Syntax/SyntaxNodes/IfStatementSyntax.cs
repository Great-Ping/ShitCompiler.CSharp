using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record IfStatementSyntax(
    Lexeme IfKeyword,
    ExpressionSyntax Condition,
    StatementSyntax ThenStatement,
    ElseClauseSyntax? ElseClause
): StatementSyntax(
    SyntaxKind.IfKeyword
) {
    public override IEnumerable<ISyntaxNode> GetChildren() {
        return new List<ISyntaxNode?>(){
            IfKeyword,
            Condition,
            ThenStatement,
            ElseClause
        }.Where(x => x is not null)!;
    }
}