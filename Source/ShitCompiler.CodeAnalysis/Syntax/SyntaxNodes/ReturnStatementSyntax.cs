using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ReturnStatementSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Keyword,
    ExpressionSyntax? Expression,
    Lexicon.Lexeme Semicolon
) : StatementSyntax(SymbolBlock, SyntaxKind.ReturnStatement)
{
    public override IEnumerable<ISyntaxNode> GetChildren()
    {
        return new List<ISyntaxNode?>(){
            Keyword,
            Expression,
            Semicolon
        }.Where(n => n is not null)!;
    }
}