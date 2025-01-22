namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public interface ISyntaxNode
{
    Location Start {get;}
    SyntaxKind Kind {get;}   
    IEnumerable<ISyntaxNode> GetChildren();
}