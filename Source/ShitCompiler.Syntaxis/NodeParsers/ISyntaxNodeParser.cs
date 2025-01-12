namespace ShitCompiler.Syntaxis.NodeParsers;

public interface ISyntaxNodeParser<TNode>
    where TNode : ISyntaxNode
{
    TNode Parse();
}