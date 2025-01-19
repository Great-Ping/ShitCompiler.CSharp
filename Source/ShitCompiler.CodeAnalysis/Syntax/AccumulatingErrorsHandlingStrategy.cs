using ShitCompiler.CodeAnalysis.Syntax.Errors;

namespace ShitCompiler.CodeAnalysis.Syntax;

public class AccumulatingErrorsHandlingStrategy(
    ICollection<ParseError> errors
): ISyntaxErrorsHandlingStrategy
{
    public void Handle<T>(T error) where T : ParseError
    {
        errors.Add(error);
    }
}