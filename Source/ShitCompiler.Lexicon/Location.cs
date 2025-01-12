namespace ShitCompiler.Lexicon;
public record Location(
    int AbsoluteIndex,
    int LineIndex,
    int SymbolIndex
);