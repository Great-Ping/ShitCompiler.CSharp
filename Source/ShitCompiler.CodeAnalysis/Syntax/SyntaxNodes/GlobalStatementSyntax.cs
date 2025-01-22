namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record GlobalStatementSyntax(
    StatementSyntax Statement
) : MemberSyntax(SyntaxKind.GlobalStatement)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return
        [
            Statement
        ];
    }
}