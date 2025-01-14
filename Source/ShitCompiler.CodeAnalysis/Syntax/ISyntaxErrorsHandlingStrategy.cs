using ShitCompiler.CodeAnalysis.Syntax.Errors;

namespace ShitCompiler.CodeAnalysis.Syntax;

public interface ISyntaxErrorsHandlingStrategy
{
    void Handle<T>(T error) where T: ParseError;  
}