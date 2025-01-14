namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record MemberSyntax(
    SymbolBlock SymbolBlock,
    SyntaxKind Kind
) : SyntaxNode(Kind);