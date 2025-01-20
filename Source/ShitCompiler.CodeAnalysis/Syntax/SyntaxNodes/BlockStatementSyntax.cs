using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record BlockStatementSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme OpenBraceToken,
    ImmutableArray<StatementSyntax> Statements,
    Lexicon.Lexeme CloseBraceToken
) : StatementSyntax(SymbolBlock, SyntaxKind.BlockStatement)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return Enumerable
            .Concat<ISyntaxNode>(
                [OpenBraceToken],
                Statements
            ).Concat(
                [CloseBraceToken]
            );
    }
}