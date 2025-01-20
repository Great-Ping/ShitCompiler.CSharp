namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public interface ISyntaxNode
{
    Dictionary<string, object> Metadata { get; }
    SyntaxKind Kind {get;}   
    IEnumerable<ISyntaxNode> GetChildren();
}