
using ShitCompiler.CodeAnalysis.Semantics;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

public record FunctionSemantic(
    TypeInfo ReturnType,
    FunctionDeclarationSyntax Function,
    List<TypeInfo> argTypes
);