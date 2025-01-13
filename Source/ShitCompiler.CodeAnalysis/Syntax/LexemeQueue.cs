using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax;

public class LexemeQueue(ILexer lexer)
{
    private int _currentIndex = 0;
    private readonly List<ParseResult<Lexeme>> _queue = new();
    private readonly ILexer _lexer = lexer;
    public int Position { get; set; }


    private ParseResult<Lexeme> ScanAndSave()
    {
        ParseResult<Lexeme> result = _lexer.ScanNext();
        _queue.Add(result);
        return result;
    }

    public ParseResult<Lexeme> Last()
    {
        if (_queue.Count == 0)
            return ScanAndSave();
        return _queue.Last();
    }
    
    public ParseResult<Lexeme> Next()
    {
        if (_currentIndex >= _queue.Count)
        {
            _currentIndex++;
            return ScanAndSave();
        }
        
        return _queue[_currentIndex];
    }
    
    public void Reset(int position)
    {
        _currentIndex = position;
    }
}