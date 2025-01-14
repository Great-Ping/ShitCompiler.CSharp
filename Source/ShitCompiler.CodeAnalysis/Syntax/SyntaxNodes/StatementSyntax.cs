namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record StatementSyntax(
    SymbolBlock SymbolBlock,
    SyntaxKind Kind
): SyntaxNode(Kind);