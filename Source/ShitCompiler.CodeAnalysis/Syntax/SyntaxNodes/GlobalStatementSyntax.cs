namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record GlobalStatementSyntax(
    SymbolBlock Block, 
    StatementSyntax Statement
) : MemberSyntax(Block, SyntaxKind.GlobalStatement);