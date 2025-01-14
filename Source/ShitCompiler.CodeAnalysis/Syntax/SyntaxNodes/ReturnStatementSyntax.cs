using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ReturnStatementSyntax(
    SymbolBlock SymbolBlock, 
    Lexeme Keyword, 
    ExpressionSyntax? expression, 
    Lexeme semicolon
) : StatementSyntax(SymbolBlock, SyntaxKind.ReturnStatement);