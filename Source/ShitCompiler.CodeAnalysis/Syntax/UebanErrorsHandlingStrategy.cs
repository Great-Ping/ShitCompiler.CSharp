using ShitCompiler.CodeAnalysis.Syntax.Errors;

namespace ShitCompiler.CodeAnalysis.Syntax;

public class ParseException(ParseError error)
    : Exception($"{error.Location.LineIndex}.{error.Location.SymbolIndex}:{error.Message}");

public class UebanErrorsHandlingStrategy: ISyntaxErrorsHandlingStrategy
{
    public void Handle<T>(T error) where T : ParseError
    {
        //Console.WriteLine("{0}.{1}: {2}", error.Location.LineIndex+1, error.Location.SymbolIndex+1, error.Message);
        throw new ParseException(error);
    }
}