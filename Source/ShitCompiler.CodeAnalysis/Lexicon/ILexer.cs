namespace ShitCompiler.CodeAnalysis.Lexicon;

public interface ILexer
{
    Lexeme ScanNext();
}