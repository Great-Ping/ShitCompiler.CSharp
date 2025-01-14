using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using ShitCompiler.CodeAnalysis.Syntax.Errors;

public record IfStatementSyntax(
    SymbolBlock SymbolBlock,
    Lexeme IfKeyword,
    ExpressionSyntax Condition,
    StatementSyntax ThenStatement,
    ElseClauseSyntax? ElseClause
): MemberSyntax(
    SymbolBlock,
    SyntaxKind.IfKeyword
) {
    public override IEnumerable<ISyntaxNode> GetChildren() {
        return [
            IfKeyword,
            Condition,
            ThenStatement,
            ElseClause
        ];
    }
}