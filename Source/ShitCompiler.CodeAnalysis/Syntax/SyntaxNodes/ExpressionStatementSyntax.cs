using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

using System.Linq;
using ShitCompiler.CodeAnalysis.Syntax.Errors;


public record ExpressionStatementSyntax(
    SymbolBlock SymbolBlock,
    ExpressionSyntax Expression
): StatementSyntax(
    SymbolBlock,
    SyntaxKind.ExpressionStatement
) {
    public override IEnumerable<ISyntaxNode> GetChildren() {
        throw new NotImplementedException(nameof(ExpressionStatementSyntax));
        return [
            // ExpressionSyntax
        ];
    }
}