using ShitCompiler.CodeAnalysis.Lexicon;
using System.Linq;
using System.Text;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public sealed record FunctionDeclarationSyntax(
    Lexeme Funk,
    Lexeme Identifier,
    Lexeme OpenParenthesisToken,
    SeparatedSyntaxList<ParameterSyntax> Parameters,
    Lexeme CloseParenthesisToken,
    TypeClauseSyntax TypeClause,
    BlockStatementSyntax Block
): MemberSyntax(SyntaxKind.FunctionDeclaration) {

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
                TypeClause,
                Block
            ]);
    }

};