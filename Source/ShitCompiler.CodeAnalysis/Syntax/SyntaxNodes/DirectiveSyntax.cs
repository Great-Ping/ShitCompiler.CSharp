namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record DirectiveSyntax(
    SymbolBlock Block
) : ISyntaxNode
{
    public SyntaxKind Kind => SyntaxKind.Directive;
    public IEnumerable<ISyntaxNode> GetChildren()
        => throw new NotImplementedException();
}