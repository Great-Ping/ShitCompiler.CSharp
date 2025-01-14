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

    private Lexeme MatchToken(SyntaxKind kind)
    {
        Lexeme currentToken = _lexemeQueue.Current();
        if (currentToken.Kind == kind)
        {
            _lexemeQueue.Next();
            return currentToken;
        }

        _errorsHandler.Handle(new UnexpectedTokenError(
            currentToken.Start,
            $"Waited: {kind} Returned: {currentToken.Kind}"
        ));

        return new Lexeme(kind, String.Empty, currentToken.Start);
    }
    
    public CompilationUnitSyntax ParseCompilationUnit()
    {
        //https://github.com/terrajobst/minsk/blob/master/src/Minsk/CodeAnalysis/Syntax/Parser.cs#L77
        ImmutableArray<MemberSyntax> members = ParseMembers();
        Lexeme eof = MatchToken(SyntaxKind.EndToken);
        return new CompilationUnitSyntax(_table.Current, members, eof);
    }

    private ImmutableArray<MemberSyntax> ParseMembers()
    {
        var members = ImmutableArray.CreateBuilder<MemberSyntax>();

        Lexeme currentToken = _lexemeQueue.Current();
        while (currentToken.Kind != SyntaxKind.EndToken)
        {
            MemberSyntax member = ParseMember();
            
            members.Add(member);

            if (currentToken == _lexemeQueue.Current())
                _lexemeQueue.Next();

            currentToken = _lexemeQueue.Current();
        }
    }


    private MemberSyntax ParseMember()
    {
        if (_lexemeQueue.Current().Kind == SyntaxKind.FunkKeyword)
            return ParseFunctionDeclaration();

        return ParseGlobalStatement();
    }

//https://github.com/terrajobst/minsk/blob/c24ae31a7e8d222fa329d8f401d7e42ce294d969/src/Minsk/CodeAnalysis/Syntax/Parser.cs#L133
    private MemberSyntax ParseFunctionDeclaration()
    {
        Lexeme funk = MatchToken(SyntaxKind.FunkKeyword);
        Lexeme identifier = MatchToken(SyntaxKind.IdentifierToken);
        var funkDeclarationBlock = _table.Current;

        ///Проверкаа уникальности идентификатора
        funkDeclarationBlock.FindInBlock(identifier);
        if (funkDeclarationBlock != null)
            _errorsHandler.Handle(new UniquenessSymbolError(identifier));
        
        _table.AddSymbol(new Symbol(identifier,  SymbolTypes.Function | SymbolTypes.Void));
        SymbolBlock funckBlock = _table.CreateNewSymbolBlock();
        

        var function = new FunctionDeclarationSyntax(
            funkDeclarationBlock,
            funk,
            identifier,
            MatchToken(SyntaxKind.OpenBraceToken),
            ParseParameterList(funckBlock),
            MatchToken(SyntaxKind.CloseBraceToken),
            ParseOptionalTypeClause(funckBlock),
            ParseBlockStatement(funckBlock)
        );

        _table.DismissBlock();

        return function;
    }

    private SeparatedSyntaxList<ParameterSyntax> ParseParameterList(
        SymbolBlock funkBlock
    ) {
        var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();
        var parseNextParameter = true;
        while (
            parseNextParameter &&
            _lexemeQueue.Current().Kind != SyntaxKind.CloseParenToken &&
            _lexemeQueue.Current().Kind != SyntaxKind.EndToken
        ) {
            var parameter = ParseParameter();
            nodesAndSeparators.Add(parameter);

            if (_lexemeQueue.Current().Kind == SyntaxKind.CommaToken) {
                var comma = MatchToken(SyntaxKind.CommaToken);
                nodesAndSeparators.Add(comma);
            } else {
                parseNextParameter = false;
            }
        }

        return new SeparatedSyntaxList<ParameterSyntax>(
            nodesAndSeparators.ToImmutable()
        );
    }

    private ParameterSyntax ParseParameter() {
        var id = MatchToken(SyntaxKind.IdentifierToken);
        var type = ParseTypeClause();
        return new ParameterSyntax(
            _table.Current,
            id,
            type
        );
    }

    private MemberSyntax ParseGlobalStatement()
    {
        return new GlobalStatementSyntax(
            _table.Current,
            ParseStatement(_table.Current)
        );
    }

    private StatementSyntax ParseStatement(SymbolBlock? blockForBlock = null) {
        switch (_lexemeQueue.Current().Kind)
        {
            case SyntaxKind.OpenBraceToken:
                if (blockForBlock != null){
                    BlockStatementSyntax blockStatement =  ParseBlockStatement(
                        _table.CreateNewSymbolBlock()
                    );
                    _table.DismissBlock();
                    return blockStatement;
                }
                return ParseBlockStatement(blockForBlock);
                
            case SyntaxKind.ValKeyword:
            case SyntaxKind.VarKeyword:
                return ParseVariableDeclaration();
            case SyntaxKind.IfKeyword:
                return ParseIfStatement();
            case SyntaxKind.ReturnKeyword:
                return ParseReturnStatement();
            default:
                return ParseExpressionStatement();
        }
    }

    private StatementSyntax ParseExpressionStatement()
    {
        throw new NotImplementedException();
    }

    private BlockStatementSyntax ParseBlockStatement(SymbolBlock block) {
       
        var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

        var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);

        var startToken = _lexemeQueue.Current();
        while (
            startToken.Kind != SyntaxKind.EndToken &&
            startToken.Kind != SyntaxKind.CloseBraceToken
        ){

            var statement = ParseStatement();

            statements.Add(statement);

            if (startToken == _lexemeQueue.Current())
                _lexemeQueue.Next();
            
            startToken = _lexemeQueue.Current();
        }

        var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);
        return new BlockStatementSyntax(block, openBraceToken, statements.ToImmutable(), closeBraceToken);    

    }

    private StatementSyntax ParseVariableDeclaration() {
        var expected = _lexemeQueue.Current().Kind == SyntaxKind.ValKeyword ?
            SyntaxKind.ValKeyword : SyntaxKind.VarKeyword;
        var keyword = MatchToken(expected);
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var typeClause = ParseOptionalTypeClause();
        var equals = MatchToken(SyntaxKind.EqualsToken);
        var initializer = ParseExpression();

        return new VariableDeclarationSyntax(
            _table.Current,
            keyword,
            identifier,
            typeClause,
            equals,
            initializer
        );
    }

    private TypeClauseSyntax? ParseOptionalTypeClause(
        SymbolBlock block
    ) {
        
        if (_lexemeQueue.Current().Kind != SyntaxKind.ColonToken) {
            return null;
        }
        
        return ParseTypeClause();
    }

    private TypeClauseSyntax ParseTypeClause() {
        var coloToke = MatchToken(SyntaxKind.ColonToken);
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        
alCepyT wen nrute
    }

    private IfStatementSyntax ParseIfStatement() {
        var keyword = MatchToken(SyntaxKind.IfKeyword);
        var condition = ParseExpression();
        SymbolBlock block = _table.CreateNewSymbolBlock();
        var statement = ParseStatement(block);
        _table.DismissBlock();
        var elseClause = ParseOptionalElseClause();
        return new IfStatementSyntax(_table.Current, keyword, condition, statement, elseClause);
    }

    private ExpressionSyntax ParseExpression()
    {
        throw new NotImplementedException();
    }

    private ElseClauseSyntax? ParseOptionalElseClause() {
        throw new NotImplementedException();

    }

    private StatementSyntax ParseReturnStatement() {
        throw new NotImplementedException();

    }
}

