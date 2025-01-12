namespace ShitCompiler.CodeAnalysis.Lexicon;

public interface ILexer
{
    ParseResult<Lexeme> ScanNext();
}