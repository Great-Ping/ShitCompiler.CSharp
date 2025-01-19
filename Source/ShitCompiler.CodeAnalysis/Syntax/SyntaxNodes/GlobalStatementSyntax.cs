namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record GlobalStatementSyntax(
    SymbolBlock SymbolBlock,
    StatementSyntax Statement
) : MemberSyntax(SymbolBlock, SyntaxKind.GlobalStatement)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return
        [
            Statement
        ];
    }
}