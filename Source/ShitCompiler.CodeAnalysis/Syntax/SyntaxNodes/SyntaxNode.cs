namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record SyntaxNode(
    SyntaxKind Kind
): ISyntaxNode
{
    public abstract IEnumerable<ISyntaxNode> GetChildren();
}