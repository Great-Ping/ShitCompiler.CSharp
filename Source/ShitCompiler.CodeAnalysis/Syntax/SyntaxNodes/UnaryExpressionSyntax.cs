using System.Diagnostics.SymbolStore;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record UnaryExpressionSyntax(
    SymbolBlock SymbolBlock, 
    Lexeme Operator, 
    ExpressionSyntax Operand
): ExpressionSyntax(SymbolBlock, SyntaxKind.UnaryExpression);