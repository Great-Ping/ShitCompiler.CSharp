using ShitCompiler.CodeAnalysis.Lexicon;
using System.Linq;
using System.Text;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record FunctionDeclarationSyntax(
    SymbolBlock SymbolBlock,
    Lexicon.Lexeme Funk,
    Lexicon.Lexeme Identifier,
    Lexicon.Lexeme OpenParenthesisToken,
    SeparatedSyntaxList<ParameterSyntax> Parameters,
    Lexicon.Lexeme CloseParenthesisToken,
    TypeClauseSyntax Type,
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
            ]);
    }

};