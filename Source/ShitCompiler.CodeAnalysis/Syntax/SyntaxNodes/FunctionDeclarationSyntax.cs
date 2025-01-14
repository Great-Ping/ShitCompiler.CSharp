using ShitCompiler.CodeAnalysis.Lexicon;
using System.Linq;
using System.Text;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record FunctionDeclarationSyntax(
    SymbolBlock SymbolBlock,
    Lexeme Funk,
    Lexeme Identifier,
    Lexeme OpenParenthesisToken,
    SeparatedSyntaxList<ParameterSyntax> Parameters,
    Lexeme CloseParenthesisToken,
    TypeClauseSyntax? Type,
    BlockStatementSyntax Block
): MemberSyntax(SymbolBlock, SyntaxKind.FunctionDeclaration) {

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return new List<SyntaxNode>(){
                Funk,
                Identifier,
                OpenParenthesisToken,
            }.Concat(
                Parameters.GetWithSeparators()
            ).Concat([
                CloseParenthesisToken,
                Type,
                Block
            ]).Where(n => n is not null)!;
    }

};