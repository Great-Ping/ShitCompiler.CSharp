using System.Collections;
using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public abstract class SeparatedSyntaxList
{
    private protected SeparatedSyntaxList()
    {
    }

    public abstract ImmutableArray<SyntaxNode> GetWithSeparators();
}

public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T>
    where T: SyntaxNode
{
    private readonly ImmutableArray<SyntaxNode> _nodesAndSeparators;

    internal SeparatedSyntaxList(ImmutableArray<SyntaxNode> nodesAndSeparators)
    {
        _nodesAndSeparators = nodesAndSeparators;
    }

    public int Count => (_nodesAndSeparators.Length + 1) / 2;

    public T this[int index] => (T) _nodesAndSeparators[index * 2];

    public Lexeme GetSeparator(int index)
    {
        if (index < 0 || index >= Count - 1)
            throw new ArgumentOutOfRangeException(nameof(index));

        return (_nodesAndSeparators[index * 2 + 1] as Lexeme)!;
    }

    public override ImmutableArray<SyntaxNode> GetWithSeparators() => _nodesAndSeparators;

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}