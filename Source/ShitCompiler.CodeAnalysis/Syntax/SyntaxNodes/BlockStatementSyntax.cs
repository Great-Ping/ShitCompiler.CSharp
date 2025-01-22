using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record BlockStatementSyntax(
    Lexeme OpenBraceToken,
    ImmutableArray<StatementSyntax> Statements,
    Lexeme CloseBraceToken
) : StatementSyntax(SyntaxKind.BlockStatement)
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