
using ShitCompiler.CodeAnalysis.Semantics;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record FunctionSemantic(
    DataType ReturnType,
    FunctionDeclarationSyntax Function,
    List<DataType> argTypes
);