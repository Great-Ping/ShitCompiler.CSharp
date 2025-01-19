namespace ShitCompiler.CodeAnalysis;

public record Location(
    int AbsoluteIndex,
    int LineIndex,
    int SymbolIndex
)
{
    public static Location Zero { get; } = new(0, 0, 0);
};