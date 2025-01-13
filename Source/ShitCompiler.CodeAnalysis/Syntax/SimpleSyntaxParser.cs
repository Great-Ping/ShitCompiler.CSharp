using System.Collections.Immutable;
using ShitCompiler.CodeAnalysis;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.Errors;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax;

public class SimpleSyntaxParser(
    LexemeQueue lexemeQueue, 
    SymbolTable table, 
    ISyntaxErrorsHandlingStrategy errorsHandler
) : ISyntaxParser {
    private readonly LexemeQueue _lexemeQueue = lexemeQueue;
    private readonly SymbolTable _table = table;
    private readonly ISyntaxErrorsHandlingStrategy _errorsHandler;

    private Lexeme NextToken(SyntaxKind kind)
    {
        Lexeme nextToken = _lexemeQueue.Next();
        if (nextToken.Kind == kind)
            return nextToken;

        _errorsHandler.Handle(new UnexpectedTokenError(
            nextToken.Start,
            $"Waited: {kind} Returned: {nextToken.Kind}"
        ));

        return new Lexeme(kind, String.Empty, nextToken.Start);
    }
    
    public CompilationUnitSyntax ParseCompilationUnit()
    {
        throw new NotImplementedException();
    }

    private ImmutableArray<DirectiveSyntax> ParseDirectives()
    {
        throw new NotImplementedException();
    }
}

