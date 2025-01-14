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
        Lexeme currentToken = _lexemeQueue.Peek();
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

        Lexeme currentToken = _lexemeQueue.Peek();
        while (currentToken.Kind != SyntaxKind.EndToken)
        {
            MemberSyntax member = ParseMember();
            
            members.Add(member);

            if (currentToken == _lexemeQueue.Peek())
                _lexemeQueue.Next();

            currentToken = _lexemeQueue.Peek();
        }
        
        return members.ToImmutable();
    }


    private MemberSyntax ParseMember()
    {
        if (_lexemeQueue.Peek().Kind == SyntaxKind.FunkKeyword)
            return ParseFunctionDeclaration();

        return ParseGlobalStatement();
    }

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
        SymbolBlock funkBlock = _table.CreateNewSymbolBlock();
        

        var function = new FunctionDeclarationSyntax(
            funkDeclarationBlock,
            funk,
            identifier,
            MatchToken(SyntaxKind.OpenBraceToken),
            ParseParameterList(),
            MatchToken(SyntaxKind.CloseBraceToken),
            ParseOptionalTypeClause(),
            ParseBlockStatement(funkBlock)
        );

        _table.DismissBlock();

        return function;
    }

    private SeparatedSyntaxList<ParameterSyntax> ParseParameterList() {
        var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();
        var parseNextParameter = true;
        while (
            parseNextParameter &&
            _lexemeQueue.Peek().Kind != SyntaxKind.CloseParenthesisToken &&
            _lexemeQueue.Peek().Kind != SyntaxKind.EndToken
        ) {
            var parameter = ParseParameter();
            nodesAndSeparators.Add(parameter);

            if (_lexemeQueue.Peek().Kind == SyntaxKind.CommaToken) {
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
        _table.AddSymbol(new Symbol(id));
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

    private StatementSyntax ParseStatement(SymbolBlock? blockForBlockStatement = null) {
        switch (_lexemeQueue.Peek().Kind)
        {
            case SyntaxKind.OpenBraceToken:
                if (blockForBlockStatement == null){
                    BlockStatementSyntax blockStatement =  ParseBlockStatement(
                        _table.CreateNewSymbolBlock()
                    );
                    _table.DismissBlock();
                    return blockStatement;
                }
                return ParseBlockStatement(blockForBlockStatement);
                
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
        return new ExpressionStatementSyntax(
            _table.Current,
            ParseExpression()
        );
    }

    private BlockStatementSyntax ParseBlockStatement(SymbolBlock block) {
       
        var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

        var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);

        var startToken = _lexemeQueue.Peek();
        while (
            startToken.Kind != SyntaxKind.EndToken &&
            startToken.Kind != SyntaxKind.CloseBraceToken
        ){

            var statement = ParseStatement();

            statements.Add(statement);

            if (startToken == _lexemeQueue.Peek())
                _lexemeQueue.Next();
            
            startToken = _lexemeQueue.Peek();
        }

        var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);
        return new BlockStatementSyntax(block, openBraceToken, statements.ToImmutable(), closeBraceToken);    

    }

    private StatementSyntax ParseVariableDeclaration() {
        var expected = _lexemeQueue.Peek().Kind == SyntaxKind.ValKeyword ?
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

    private TypeClauseSyntax? ParseOptionalTypeClause() {
        if (_lexemeQueue.Peek().Kind != SyntaxKind.ColonToken) {
            return null;
        }
        
        return ParseTypeClause();
    }

    private TypeClauseSyntax ParseTypeClause() {
        var coloToke = MatchToken(SyntaxKind.ColonToken);
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        
        return new TypeClauseSyntax(
            _table.Current,
            coloToke,
            identifier
        );
    }

    private IfStatementSyntax ParseIfStatement() {
        var keyword = MatchToken(SyntaxKind.IfKeyword);
        var condition = ParseExpression();
        
        //Контекст для блока внутри If
        SymbolBlock attachmentsBlock = _table.CreateNewSymbolBlock();
        var statement = ParseStatement(attachmentsBlock);
        _table.DismissBlock();
        
        var elseClause = ParseOptionalElseClause();
        return new IfStatementSyntax(_table.Current, keyword, condition, statement, elseClause);
    }

    private ElseClauseSyntax? ParseOptionalElseClause() {
        if (_lexemeQueue.Peek().Kind != SyntaxKind.ElseKeyword)
            return null;

        var keyword = _lexemeQueue.Next();
        
        SymbolBlock attachmentsBlock = _table.CreateNewSymbolBlock();
        var statement = ParseStatement(attachmentsBlock);
        _table.DismissBlock();

        return new ElseClauseSyntax(
            _table.Current,
            keyword,
            statement
        );
    }

    private StatementSyntax ParseReturnStatement() {
        throw new NotImplementedException();

    }
    
    private ExpressionSyntax ParseExpression()
    {
        if (_lexemeQueue.Peek().Kind == SyntaxKind.IdentifierToken && 
            _lexemeQueue.Peek(1).Kind == SyntaxKind.EqualsToken)
            return ParseAssignmentExpression();
        
        return ParseBinaryExpression();
    }


    private ExpressionSyntax ParseAssignmentExpression()
    {
        var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
        var operatorToken = MatchToken(SyntaxKind.EqualsToken);
        var right = ParseBinaryExpression();
        return new AssignmentExpressionSyntax(_table.Current, identifierToken, operatorToken, right);
    }

    private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedence = _lexemeQueue.Peek().Kind.GetUnaryOperatorPrecedence();
        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
        {
            var operatorToken = NextToken();
            var operand = ParseBinaryExpression(unaryOperatorPrecedence);
            left = new UnaryExpressionSyntax(_syntaxTree, operatorToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            var precedence = Current.Kind.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            var operatorToken = NextToken();
            var right = ParseBinaryExpression(precedence);
            left = new BinaryExpressionSyntax(_syntaxTree, left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.OpenParenthesisToken:
                return ParseParenthesizedExpression();
            
            case SyntaxKind.FalseKeyword:
            case SyntaxKind.TrueKeyword:
                return ParseBooleanLiteral();

            case SyntaxKind.NumberToken:
                return ParseNumberLiteral();

            case SyntaxKind.StringToken:
                return ParseStringLiteral();

            case SyntaxKind.IdentifierToken:
            default:
                return ParseNameOrCallExpression();
        }
    }

    private ExpressionSyntax ParseParenthesizedExpression()
    {
        var left = MatchToken(SyntaxKind.OpenParenthesisToken);
        var expression = ParseExpression();
        var right = MatchToken(SyntaxKind.CloseParenthesisToken);
        return new ParenthesizedExpressionSyntax(_syntaxTree, left, expression, right);
    }

    private ExpressionSyntax ParseBooleanLiteral()
    {
        var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
        var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
        return new LiteralExpressionSyntax(_syntaxTree, keywordToken, isTrue);
    }

    private ExpressionSyntax ParseNumberLiteral()
    {
        var numberToken = MatchToken(SyntaxKind.NumberToken);
        return new LiteralExpressionSyntax(_syntaxTree, numberToken);
    }

    private ExpressionSyntax ParseStringLiteral()
    {
        var stringToken = MatchToken(SyntaxKind.StringToken);
        return new LiteralExpressionSyntax(_syntaxTree, stringToken);
    }

    private ExpressionSyntax ParseNameOrCallExpression()
    {
        if (Peek(0).Kind == SyntaxKind.IdentifierToken && Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
            return ParseCallExpression();

        return ParseNameExpression();
    }

    private ExpressionSyntax ParseCallExpression()
    {
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
        var arguments = ParseArguments();
        var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
        return new CallExpressionSyntax(_syntaxTree, identifier, openParenthesisToken, arguments, closeParenthesisToken);
    }

    private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
    {
        var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

        var parseNextArgument = true;
        while (parseNextArgument &&
               Current.Kind != SyntaxKind.CloseParenthesisToken &&
               Current.Kind != SyntaxKind.EndOfFileToken)
        {
            var expression = ParseExpression();
            nodesAndSeparators.Add(expression);

            if (Current.Kind == SyntaxKind.CommaToken)
            {
                var comma = MatchToken(SyntaxKind.CommaToken);
                nodesAndSeparators.Add(comma);
            }
            else
            {
                parseNextArgument = false;
            }
        }

        return new SeparatedSyntaxList<ExpressionSyntax>(nodesAndSeparators.ToImmutable());
    }

    private ExpressionSyntax ParseNameExpression()
    {
        var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
        return new NameExpressionSyntax(_syntaxTree, identifierToken);
    }
}

