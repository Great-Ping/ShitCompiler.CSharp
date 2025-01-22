using ShitCompiler.CodeAnalysis.Syntax.Errors;

namespace ShitCompiler.CodeAnalysis.Syntax.ErrorHandlers;

public class AccumulatingErrorsHandler(
    ICollection<ParseError> errors
): ISyntaxErrorsHandler
{
    public void Handle<T>(T error) where T : ParseError
    {
        errors.Add(error);
    }
}