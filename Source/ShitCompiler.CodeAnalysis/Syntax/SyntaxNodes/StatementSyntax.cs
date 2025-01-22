namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record StatementSyntax(
    SyntaxKind Kind
): SyntaxNode(Kind);