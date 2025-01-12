using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax.NodeParsers;

public interface ISyntaxNodeParser<TNode>
    where TNode : ISyntaxNode
{
    ParseResult<TNode> Parse();
}