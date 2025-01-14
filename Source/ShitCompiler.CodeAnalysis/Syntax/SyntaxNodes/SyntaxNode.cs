namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record SyntaxNode(
    SyntaxKind Kind
): ISyntaxNode
{
    public virtual IEnumerable<ISyntaxNode> GetChildren()
        => [];
}