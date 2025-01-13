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
    
    public ParseResult<CompilationUnitSyntax> ParseCompilationUnit()
    {
        ParseResult<ImmutableArray<DirectiveSyntax>> resultDirectives = ParseDirectives();
        if (resultDirectives.TryFailure(out var directives, out var error))
            return error;
        
        ParseResult<Lexeme> resultEndToken = _lexemeQueue.Next();
        if (resultEndToken.TryFailure(out var endToken, out var errorEndToken))
            return errorEndToken;

        if (endToken.Kind != SyntaxKind.EndToken)
            return ErrorHelper.AnotherKindExpected(endToken.Start, SyntaxKind.EndToken, endToken.Kind);

        return new CompilationUnitSyntax(_table.Current, directives, endToken);
    }

    private ParseResult<ImmutableArray<DirectiveSyntax>> ParseDirectives()
    { 
        ImmutableArray<DirectiveSyntax>.Builder directives = ImmutableArray.CreateBuilder<DirectiveSyntax>();
        
        ParseResult<Lexeme> result = _lexemeQueue.Last();
        if (result.TryFailure(out var lexeme, out var error))
            return error;
        
        while (lexeme.Kind != SyntaxKind.EndToken)
        {
            
            ParseResult<DirectiveSyntax> directiveParseResult = _lexemeQueue.Next();
            directives.Add();
            
            result = _lexemeQueue.Last();
            if (result.TryFailure(out lexeme, out error))
                return error;
        }
    }
}