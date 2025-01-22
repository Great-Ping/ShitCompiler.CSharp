namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public interface ISyntaxNode
{
    SyntaxKind Kind {get;}   
    IEnumerable<ISyntaxNode> GetChildren();
}