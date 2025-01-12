namespace ShitCompiler.CodeAnalysis;
public record Location(
    int AbsoluteIndex,
    int LineIndex,
    int SymbolIndex
);