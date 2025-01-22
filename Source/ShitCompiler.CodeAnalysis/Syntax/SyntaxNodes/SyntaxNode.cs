namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record SyntaxNode(
    SyntaxKind Kind
): ISyntaxNode
{

    public virtual Location Start => GetChildren().First().Start;
    public abstract IEnumerable<ISyntaxNode> GetChildren();
}