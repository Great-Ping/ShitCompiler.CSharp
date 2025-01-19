using System.Collections;
using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;
public sealed record CompilationUnitSyntax(
    SymbolBlock Block,
    ImmutableArray<MemberSyntax> Members,
    Lexeme EndOfFileToken
) : ISyntaxNode
{
    public SyntaxKind Kind => SyntaxKind.CompilationUnit;
    public IEnumerable<ISyntaxNode> GetChildren()
        => Members;

    public IEnumerable<Lexeme> GetLexemes()
    {
        List<Lexeme> lexemes = new List<Lexeme>();
        List<ISyntaxNode> syntaxNodes = GetChildren().ToList();

        while (syntaxNodes.Count > 0)
        {
            ISyntaxNode node = syntaxNodes.First();
            syntaxNodes.RemoveAt(0);

            if (node is Lexeme lexeme)
            {
                lexemes.Add(lexeme);
                continue;
            }
            
            IEnumerable<ISyntaxNode> children = node.GetChildren();

            foreach (ISyntaxNode child in children.Reverse())
            {
                syntaxNodes.Insert(0, child);
            }
        }
        
        return lexemes;
    }
};