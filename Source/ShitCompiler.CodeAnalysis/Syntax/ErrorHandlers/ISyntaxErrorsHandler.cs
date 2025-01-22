using ShitCompiler.CodeAnalysis.Syntax.Errors;

namespace ShitCompiler.CodeAnalysis.Syntax.ErrorHandlers;

public interface ISyntaxErrorsHandler
{
    void Handle<T>(T error) where T: ParseError;  
}