namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record SyntaxNode(
    SyntaxKind Kind
): ISyntaxNode
{
    public Dictionary<string, object> Metadata { get; } = new();
    public abstract IEnumerable<ISyntaxNode> GetChildren();
}