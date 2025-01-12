namespace ShitCompiler.Lexicon;

public record Lexeme(
    LexemeKind Kind, 
    ReadOnlyMemory<char> StringValue,
    Location Start
);

public record Lexeme<T>(
    LexemeKind Kind, 
    ReadOnlyMemory<char> StringValue,
    Location Start,
    T ParsedValue
): Lexeme(Kind, StringValue, Start);

public record InvalidLexeme(
    ErrorCode ErrorCode,
    ReadOnlyMemory<char> StringValue,
    Location Start
): Lexeme(LexemeKind.InvalidToken, StringValue, Start);