using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record BlockStatementSyntax(
    SymbolBlock SymbolBlock,
    Lexeme OpenBraceToken, 
    ImmutableArray<StatementSyntax> Statements, 
    Lexeme CloseBraceToken
): StatementSyntax(SymbolBlock, SyntaxKind.BlockStatement);