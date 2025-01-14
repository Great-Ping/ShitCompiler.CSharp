namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record GlobalStatementSyntax(
    SymbolBlock Block, 
    StatementSyntax Statement
) : MemberSyntax(Block, SyntaxKind.GlobalStatement);