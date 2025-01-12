namespace ShitCompiler.Lexicon;

public interface ILexer
{
    Lexeme ScanNext();
}