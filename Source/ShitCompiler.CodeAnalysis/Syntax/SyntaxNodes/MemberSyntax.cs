namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record MemberSyntax(
    SyntaxKind Kind
) : SyntaxNode(Kind);