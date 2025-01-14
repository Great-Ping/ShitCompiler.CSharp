using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax.Errors;

public record ExpressionSyntax(
    SyntaxKind Kind
) : SyntaxNode(Kind){
    
}