using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax;

public class LexemeQueue(ILexer lexer)
{
    private int _currentIndex = 0;
    private readonly List<Lexeme> _queue = new();
    private readonly ILexer _lexer = lexer;
    public int CurrentIndex => _currentIndex;


    private Lexeme ScanAndSave()
    {
        Lexeme result = _lexer.ScanNext();
        _queue.Add(result);
        return result;
    }
    
    public Lexeme Peek(int index = 0)
    {
        for (int i = _currentIndex + index; i >= _queue.Count; i--)
            ScanAndSave();

        return _queue[_currentIndex + index];
    }

    
    public Lexeme Next()
    {
        _currentIndex++;

        if (_currentIndex >= _queue.Count)
            return ScanAndSave();
        
        return _queue[_currentIndex];
    }

    public void Reset(int position)
    {
        _currentIndex = position;
    }
}