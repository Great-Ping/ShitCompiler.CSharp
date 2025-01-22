using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record ReturnStatementSyntax(
    Lexeme Keyword,
    ExpressionSyntax? Expression,
    Lexeme Semicolon
) : StatementSyntax(SyntaxKind.ReturnStatement)
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