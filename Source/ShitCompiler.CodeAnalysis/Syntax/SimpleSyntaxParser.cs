using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.Errors;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax;

public class SimpleSyntaxParser(LexemeQueue lexemeQueue, SymbolTable table) : ISyntaxParser
{
    private readonly LexemeQueue _lexemeQueue = lexemeQueue;
    private readonly SymbolTable _table = table;


    public CompilationUnitSyntax ParseCompilationUnit()
    {
        throw new NotImplementedException();
    }

    private ImmutableArray<DirectiveSyntax> ParseDirectives()
    {
        throw new NotImplementedException();
    }
}