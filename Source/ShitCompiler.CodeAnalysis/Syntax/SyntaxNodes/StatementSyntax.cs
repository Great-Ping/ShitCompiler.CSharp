namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record StatementSyntax(
    SymbolBlock SymbolBlock,
    SyntaxKind Kind
): SyntaxNode(Kind);