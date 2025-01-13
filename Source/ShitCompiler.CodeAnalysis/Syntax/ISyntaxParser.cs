using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax;

public interface ISyntaxParser
{
    public ParseResult<CompilationUnitSyntax> ParseCompilationUnit();   
}