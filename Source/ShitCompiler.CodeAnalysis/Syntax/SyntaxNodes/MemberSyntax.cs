namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record MemberSyntax(
    SymbolBlock SymbolBlock,
    SyntaxKind Kind
) : SyntaxNode(Kind);