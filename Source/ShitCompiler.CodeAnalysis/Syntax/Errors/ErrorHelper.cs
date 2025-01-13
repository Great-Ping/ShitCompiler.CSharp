using ShitCompiler.CodeAnalysis.Errors;

namespace ShitCompiler.CodeAnalysis.Syntax.Errors;

static partial class ErrorHelper
{
    public static ParseError AnotherKindExpected(
        Location location, 
        SyntaxKind expectedKind, 
        SyntaxKind  receivedKind
    ){
        return new UnexpectedTokenError(
            location, 
            $"Expected:{expectedKind} Received:{receivedKind}"
        );
    }
}