namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record ExpressionSyntax(
    SymbolBlock SymbolBlock,
    SyntaxKind Kind
) : SyntaxNode(Kind){
    
}