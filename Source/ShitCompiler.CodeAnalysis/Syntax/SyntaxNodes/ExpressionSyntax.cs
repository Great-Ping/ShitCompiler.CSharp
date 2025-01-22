namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract record ExpressionSyntax(
    SyntaxKind Kind
) : SyntaxNode(Kind){
    
}