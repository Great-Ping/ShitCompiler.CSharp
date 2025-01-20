using System.Collections;
using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;
public sealed record CompilationUnitSyntax(
    SymbolBlock Block,
    ImmutableArray<MemberSyntax> Members,
    Lexicon.Lexeme EndOfFileToken
) : SyntaxNode(SyntaxKind.CompilationUnit)
{
    public static CompilationUnitSyntax Empty { get; } = new(
        new SymbolBlock(
            null,
            ImmutableDictionary<string, Lexeme>.Empty
        ),
        ImmutableArray<MemberSyntax>.Empty,
        new Lexicon.Lexeme(SyntaxKind.EndToken, String.Empty, Location.Zero)
    );

    public override IEnumerable<ISyntaxNode> GetChildren()
        => Members;

    public IEnumerable<Lexicon.Lexeme> GetLexemes()
    {
        List<Lexicon.Lexeme> lexemes = new List<Lexicon.Lexeme>();
        List<ISyntaxNode> syntaxNodes = GetChildren().ToList();

        while (syntaxNodes.Count > 0)
        {
            ISyntaxNode node = syntaxNodes.First();
            syntaxNodes.RemoveAt(0);

            if (node is Lexicon.Lexeme lexeme)
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