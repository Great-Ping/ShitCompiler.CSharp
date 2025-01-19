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
    ISyntaxErrorsHandler errorsHandler
) : ISyntaxParser
{
    private readonly LexemeQueue _lexemeQueue = lexemeQueue;
    private readonly SymbolTable _table = table;
    private readonly ISyntaxErrorsHandler _errorsHandler = errorsHandler;

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

    private Lexeme MatchToken(Func<SyntaxKind, bool> check, SyntaxKind asDefault)
    {
        Lexeme currentToken = _lexemeQueue.Peek();
        if (check(currentToken.Kind))
        {
            _lexemeQueue.Next();
            return currentToken;
        }

        _errorsHandler.Handle(new UnexpectedTokenError(
            currentToken.Start,
            $"Waited: {asDefault} Returned: {currentToken.Kind}"
        ));

        return new Lexeme(asDefault, String.Empty, currentToken.Start);
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
        var symbol = funkDeclarationBlock.FindInBlock(identifier);
        if (symbol != null)
            _errorsHandler.Handle(new UniquenessSymbolError(identifier));

        _table.AddSymbol(new Symbol(identifier, SymbolTypes.Function | SymbolTypes.Void));
        SymbolBlock funkBlock = _table.CreateNewSymbolBlock();


        var function = new FunctionDeclarationSyntax(
            funkDeclarationBlock,
            funk,
            identifier,
            MatchToken(SyntaxKind.OpenParenthesisToken),
            ParseParameterList(),
            MatchToken(SyntaxKind.CloseParenthesisToken),
            ParseTypeClause(),
            ParseBlockStatement(funkBlock)
        );

        _table.DismissBlock();

        return function;
    }

    private SeparatedSyntaxList<ParameterSyntax> ParseParameterList()
    {
        var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();
        var parseNextParameter = true;
        while (
            parseNextParameter &&
            _lexemeQueue.Peek().Kind != SyntaxKind.CloseParenthesisToken &&
            _lexemeQueue.Peek().Kind != SyntaxKind.EndToken
        )
        {
            var parameter = ParseParameter();
            nodesAndSeparators.Add(parameter);

            if (_lexemeQueue.Peek().Kind == SyntaxKind.CommaToken)
            {
                var comma = MatchToken(SyntaxKind.CommaToken);
                nodesAndSeparators.Add(comma);
            }
            else
            {
                parseNextParameter = false;
            }
        }

        return new SeparatedSyntaxList<ParameterSyntax>(
            nodesAndSeparators.ToImmutable()
        );
    }

    private ParameterSyntax ParseParameter()
    {
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

    private StatementSyntax ParseStatement(SymbolBlock? blockForBlockStatement = null)
    {
        switch (_lexemeQueue.Peek().Kind)
        {
            case SyntaxKind.OpenBraceToken:
                if (blockForBlockStatement == null)
                {
                    BlockStatementSyntax blockStatement = ParseBlockStatement(
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
        var expression = ParseExpression();
        var semicolon = MatchToken(SyntaxKind.SemicolonToken);
        return new ExpressionStatementSyntax(
            _table.Current,
            expression,
            semicolon
        );
    }

    private BlockStatementSyntax ParseBlockStatement(SymbolBlock block)
    {

        var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

        var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);

        var startToken = _lexemeQueue.Peek();
        while (
            startToken.Kind != SyntaxKind.EndToken &&
            startToken.Kind != SyntaxKind.CloseBraceToken
        )
        {

            var statement = ParseStatement();

            statements.Add(statement);

            if (startToken == _lexemeQueue.Peek())
                _lexemeQueue.Next();

            startToken = _lexemeQueue.Peek();
        }

        var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);
        return new BlockStatementSyntax(block, openBraceToken, statements.ToImmutable(), closeBraceToken);

    }

    private StatementSyntax ParseVariableDeclaration()
    {
        var expected = _lexemeQueue.Peek().Kind == SyntaxKind.ValKeyword
            ? SyntaxKind.ValKeyword
            : SyntaxKind.VarKeyword;
        
        var keyword = MatchToken(expected);

        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        _table.AddSymbol(new(identifier));
        
        var typeClause = ParseTypeClause();
        var equals = MatchToken(SyntaxKind.EqualsToken);
        var initializer = ParseExpression();
        var semicolon = MatchToken(SyntaxKind.SemicolonToken);

        return new VariableDeclarationSyntax(
            _table.Current,
            keyword,
            identifier,
            typeClause,
            equals,
            initializer,
            semicolon
        );
    }


    //TODO
    //Добавить ебаный семантический анализ
    //и проверку на массив
    private TypeClauseSyntax ParseTypeClause()
    {
        var coloToke = MatchToken(SyntaxKind.ColonToken);
      
        return new TypeClauseSyntax(
            _table.Current,
            coloToke,
            ParseType()
        );
    }

    private TypeSyntax ParseType()
    {
        var type = MatchToken((kind) => kind.IsType(), SyntaxKind.Type);

        if (_lexemeQueue.Peek(1).Kind == SyntaxKind.OpenBracketToken)
        {
            var openBracket = MatchToken(SyntaxKind.OpenBracketToken);
            var number = (MatchToken(SyntaxKind.NumberToken) as Lexeme<long>)!;
            var closeBracket = MatchToken(SyntaxKind.CloseBracketToken);
            return new ArrayTypeSyntax(_table.Current, type, openBracket, number, closeBracket);
        }
        
        return new IdentifierTypeSyntax(_table.Current, type);

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

        var keyword = _lexemeQueue.Peek();
        _lexemeQueue.Next();
        
        //Контекст внутри блока else
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
        var keyword = MatchToken(SyntaxKind.ReturnKeyword);
        ExpressionSyntax? expression = null;
        
        if (_lexemeQueue.Peek().Kind != SyntaxKind.SemicolonToken)
            expression = ParseExpression();
        
        var semicolon = MatchToken(SyntaxKind.SemicolonToken);

        return new ReturnStatementSyntax(
            _table.Current, 
            keyword, 
            expression, 
            semicolon
        );
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
            var operatorToken = _lexemeQueue.Peek();
            _lexemeQueue.Next();
            var operand = ParseBinaryExpression(unaryOperatorPrecedence);
            left = new UnaryExpressionSyntax(_table.Current, operatorToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            var precedence = _lexemeQueue.Peek().Kind.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            var operatorToken = _lexemeQueue.Peek();
            _lexemeQueue.Next();
            var right = ParseBinaryExpression(precedence);
            left = new BinaryExpressionSyntax(_table.Current, left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (_lexemeQueue.Peek().Kind)
        {
            case SyntaxKind.OpenParenthesisToken:
                return ParseParenthesizedExpression();
            
            case SyntaxKind.FalseKeyword:
            case SyntaxKind.TrueKeyword:
                return ParseBooleanLiteral();

            case SyntaxKind.NumberToken:
                return ParseNumberLiteral();
            case SyntaxKind.RealNumberToken:
                return ParseRealNumberLiteral();
            
            case SyntaxKind.StringToken:
                return ParseStringLiteral();
            case SyntaxKind.CharacterToken:
                return ParseCharLiteral();
            
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
        return new ParenthesizedExpressionSyntax(_table.Current, left, expression, right);
    }

    private ExpressionSyntax ParseBooleanLiteral()
    {
        var isTrue = _lexemeQueue.Peek().Kind == SyntaxKind.TrueKeyword;
        var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
        return new LiteralExpressionSyntax<bool>(_table.Current, keywordToken, isTrue);
    }
    private ExpressionSyntax ParseRealNumberLiteral()
    {
        var numberToken = (MatchToken(SyntaxKind.NumberToken) as Lexeme<long>)!;
        return new LiteralExpressionSyntax<double>(_table.Current, numberToken, numberToken.ParsedValue);
    }

    private ExpressionSyntax ParseNumberLiteral()
    {
        var numberToken = (MatchToken(SyntaxKind.NumberToken) as Lexeme<long>)!;
        return new LiteralExpressionSyntax<long>(_table.Current, numberToken, numberToken.ParsedValue);
    }

    private ExpressionSyntax ParseStringLiteral()
    {
        var stringToken = (MatchToken(SyntaxKind.StringToken) as Lexeme<string>)!;
        return new LiteralExpressionSyntax<string>(_table.Current, stringToken, stringToken.ParsedValue);
    }

    private ExpressionSyntax ParseCharLiteral()
    {
        var stringToken = (MatchToken(SyntaxKind.CharacterToken) as Lexeme<char>)!;
        return new LiteralExpressionSyntax<char>(_table.Current, stringToken, stringToken.ParsedValue);
    }

    private ExpressionSyntax ParseNameOrCallExpression()
    {
        if (_lexemeQueue.Peek().Kind == SyntaxKind.IdentifierToken
            && _lexemeQueue.Peek(1).Kind == SyntaxKind.OpenParenthesisToken
        ) return ParseCallExpression();

        return ParseNameExpression();
    }

    private ExpressionSyntax ParseCallExpression()
    {
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
        var arguments = ParseArguments();
        var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
        return new CallExpressionSyntax(_table.Current, identifier, openParenthesisToken, arguments, closeParenthesisToken);
    }

    private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
    {
        var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

        var parseNextArgument = true;
        while (parseNextArgument &&
               _lexemeQueue.Peek().Kind != SyntaxKind.CloseParenthesisToken &&
               _lexemeQueue.Peek().Kind != SyntaxKind.EndToken)
        {
            var expression = ParseExpression();
            nodesAndSeparators.Add(expression);

            if (_lexemeQueue.Peek().Kind == SyntaxKind.CommaToken)
            {
                var comma = MatchToken(SyntaxKind.CommaToken);
                nodesAndSeparators.Add(comma);
            }
            else
            {
                parseNextArgument = false;
            }
        }

        return new SeparatedSyntaxList<ExpressionSyntax>(
            nodesAndSeparators.ToImmutable()
        );
    }

    private ExpressionSyntax ParseNameExpression()
    {
        var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
        return new NameExpressionSyntax(_table.Current, identifierToken);
    }
}
