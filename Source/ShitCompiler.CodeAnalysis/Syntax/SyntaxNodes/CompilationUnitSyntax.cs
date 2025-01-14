using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;
public sealed record CompilationUnitSyntax(
    SymbolBlock Block,
    ImmutableArray<MemberSyntax> Directives,
    Lexeme EndOfFileToken
) : ISyntaxNode
{
    public SyntaxKind Kind => SyntaxKind.CompilationUnitSyntax;
    public IEnumerable<ISyntaxNode> GetChildren()
        => Directives;
};