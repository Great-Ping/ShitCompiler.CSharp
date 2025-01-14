namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record TypeClauseSyntax(
    SyntaxKind Kind
) : SyntaxNode(Kind);