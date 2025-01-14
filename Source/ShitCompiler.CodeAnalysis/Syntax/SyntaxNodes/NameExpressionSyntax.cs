using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record NameExpressionSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Identifier
) : ExpressionSyntax(SymbolBlock, SyntaxKind.NameExpression);