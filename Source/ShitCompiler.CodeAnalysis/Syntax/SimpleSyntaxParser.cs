using System.Collections.Immutable;

using ShitCompiler.CodeAnalysis;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.Errors;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax;

public class SimpleSyntaxParser(
    LexemeQueue lexemeQueue,
    ISyntaxErrorsHandler errorsHandler
) : ISyntaxParser
{
    private readonly LexemeQueue _lexemeQueue = lexemeQueue;
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

    public CompilationUnitSyntax ParseCompilationUnit()
    {
        //https://github.com/terrajobst/minsk/blob/master/src/Minsk/CodeAnalysis/Syntax/Parser.cs#L77
        ImmutableArray<MemberSyntax> members = ParseMembers();
        Lexeme eof = MatchToken(SyntaxKind.EndToken);
        return new CompilationUnitSyntax(members, eof);
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

        var function = new FunctionDeclarationSyntax(
            funk,
            identifier,
            MatchToken(SyntaxKind.OpenParenthesisToken),
            ParseParameterList(),
            MatchToken(SyntaxKind.CloseParenthesisToken),
            ParseTypeClause(),
            ParseBlockStatement()
        );

     
        return function;
    }

    private ArrayExpressionSyntax ParseArrayExpression()
    {
        return new ArrayExpressionSyntax(
            
            SyntaxKind.ArrayExpression,
            MatchToken(SyntaxKind.OpenBracketToken),
            ParseArguments(SyntaxKind.OpenBracketToken),
            MatchToken(SyntaxKind.CloseBracketToken)
        );

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
        var type = ParseTypeClause();
        return new ParameterSyntax(
            id,
            type
        );
    }

    private MemberSyntax ParseGlobalStatement()
    {
        return new GlobalStatementSyntax(
            
            ParseStatement()
        );
    }

    private StatementSyntax ParseStatement()
    {
        switch (_lexemeQueue.Peek().Kind)
        {
            case SyntaxKind.OpenBraceToken:
                return ParseBlockStatement();
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
            
            expression,
            semicolon
        );
    }

    private BlockStatementSyntax ParseBlockStatement()
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
        return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);

    }

    private StatementSyntax ParseVariableDeclaration()
    {
        var expected = _lexemeQueue.Peek().Kind == SyntaxKind.ValKeyword
            ? SyntaxKind.ValKeyword
            : SyntaxKind.VarKeyword;

        var keyword = MatchToken(expected);

        var identifier = MatchToken(SyntaxKind.IdentifierToken);

        var typeClause = ParseTypeClause();
        var equals = MatchToken(SyntaxKind.EqualsToken);
        var initializer = ParseBinaryExpression();
        var semicolon = MatchToken(SyntaxKind.SemicolonToken);

        return new VariableDeclarationSyntax(
            
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
            
            coloToke,
            ParseType()
        );
    }

    private TypeSyntax ParseType()
    {
        var type = MatchToken(SyntaxKind.IdentifierToken);

        if (_lexemeQueue.Peek().Kind == SyntaxKind.OpenBracketToken)
        {
            var openBracket = MatchToken(SyntaxKind.OpenBracketToken);
            var number = ParseNumberLiteral();
            var closeBracket = MatchToken(SyntaxKind.CloseBracketToken);
            return new ArrayTypeSyntax(type, openBracket, number, closeBracket);
        }

        return new IdentifierTypeSyntax(type);

    }

    private IfStatementSyntax ParseIfStatement()
    {
        var keyword = MatchToken(SyntaxKind.IfKeyword);
        var condition = ParseExpression();
        var statement = ParseStatement();
        var elseClause = ParseOptionalElseClause();
        return new IfStatementSyntax(keyword, condition, statement, elseClause);
    }

    private ElseClauseSyntax? ParseOptionalElseClause()
    {
        if (_lexemeQueue.Peek().Kind != SyntaxKind.ElseKeyword)
            return null;

        var keyword = _lexemeQueue.Peek();
        _lexemeQueue.Next();

        //Контекст внутри блока else
        var statement = ParseStatement();

        return new ElseClauseSyntax(
            
            keyword,
            statement
        );
    }

    private StatementSyntax ParseReturnStatement()
    {
        var keyword = MatchToken(SyntaxKind.ReturnKeyword);
        ExpressionSyntax? expression = null;

        if (_lexemeQueue.Peek().Kind != SyntaxKind.SemicolonToken)
            expression = ParseExpression();

        var semicolon = MatchToken(SyntaxKind.SemicolonToken);

        return new ReturnStatementSyntax(
            
            keyword,
            expression,
            semicolon
        );
    }

    private ExpressionSyntax ParseExpression()
    {
        if (_lexemeQueue.Peek().Kind == SyntaxKind.IdentifierToken &&
            _lexemeQueue.Peek(1).Kind == SyntaxKind.EqualsToken ||
            _lexemeQueue.Peek(1).Kind == SyntaxKind.OpenBracketToken)
            return ParseAssignmentExpression();

        if (_lexemeQueue.Peek().Kind == SyntaxKind.OpenBracketToken)
            return ParseArrayExpression();

        return ParseBinaryExpression();
    }


    private ExpressionSyntax ParseAssignmentExpression()
    {
        var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
        Lexeme operatorToken;
        ExpressionSyntax right;
        if (_lexemeQueue.Peek().Kind == SyntaxKind.OpenBracketToken)
        {
            var openBracket = MatchToken(SyntaxKind.OpenBracketToken);
            var expression = ParseBinaryExpression();
            var closeBracket = MatchToken(SyntaxKind.CloseBracketToken);
            operatorToken = MatchToken(SyntaxKind.EqualsToken);
            right = ParseBinaryExpression();
            return new ArrayAssigmentExpressionSyntax(
                
                identifierToken,
                openBracket,
                expression,
                closeBracket,
                operatorToken,
                right
            );
        }

        operatorToken = MatchToken(SyntaxKind.EqualsToken);
        right = ParseBinaryExpression();
        return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
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
            left = new UnaryExpressionSyntax(operatorToken, operand);
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
            left = new BinaryExpressionSyntax(left, operatorToken, right);
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
                return ParseNameOrCallOrIndexExpression();
        }
    }


    private ExpressionSyntax ParseParenthesizedExpression()
    {
        var left = MatchToken(SyntaxKind.OpenParenthesisToken);
        var expression = ParseBinaryExpression();
        var right = MatchToken(SyntaxKind.CloseParenthesisToken);
        return new ParenthesizedExpressionSyntax(left, expression, right);
    }

    private LiteralExpressionSyntax<bool> ParseBooleanLiteral()
    {
        var isTrue = _lexemeQueue.Peek().Kind == SyntaxKind.TrueKeyword;
        var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
        return new LiteralExpressionSyntax<bool>(keywordToken, isTrue);
    }
    private LiteralExpressionSyntax<double> ParseRealNumberLiteral()
    {
        var token = MatchToken(SyntaxKind.RealNumberToken);
        var numberToken = token as Lexeme<double>;
        return new LiteralExpressionSyntax<double>(token, numberToken?.ParsedValue ?? 0.0d);
    }

    private LiteralExpressionSyntax<long> ParseNumberLiteral()
    {
        var token = MatchToken(SyntaxKind.NumberToken);
        var numberToken = token as Lexeme<long>;
        return new LiteralExpressionSyntax<long>(token, numberToken?.ParsedValue ?? 0);
    }

    private LiteralExpressionSyntax<string> ParseStringLiteral()
    {
        var token = MatchToken(SyntaxKind.StringToken);
        var stringToken = token as Lexeme<string>;
        return new LiteralExpressionSyntax<string>(token, stringToken?.ParsedValue ?? string.Empty);
    }

    private ExpressionSyntax ParseCharLiteral()
    {
        var token = MatchToken(SyntaxKind.CharacterToken);
        var charToken = token as Lexeme<char>;
        return new LiteralExpressionSyntax<char>(token, charToken?.ParsedValue ?? TextCursor.InvalidCharacter);
    }

    private ExpressionSyntax ParseNameOrCallOrIndexExpression()
    {
        if (_lexemeQueue.Peek().Kind == SyntaxKind.IdentifierToken
            && _lexemeQueue.Peek(1).Kind == SyntaxKind.OpenParenthesisToken
        ) return ParseCallExpression();

        if (_lexemeQueue.Peek().Kind == SyntaxKind.IdentifierToken
            && _lexemeQueue.Peek(1).Kind == SyntaxKind.OpenBracketToken
           ) return ParseIndexExpression();


        return ParseNameExpression();
    }

    private ExpressionSyntax ParseIndexExpression()
    {
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var openParenthesisToken = MatchToken(SyntaxKind.OpenBracketToken);
        var arguments = ParseArguments(SyntaxKind.CloseBracketToken);
        var closeParenthesisToken = MatchToken(SyntaxKind.CloseBracketToken);
        return new IndexExpressionSyntax(identifier, openParenthesisToken, arguments, closeParenthesisToken);
    }

    private ExpressionSyntax ParseCallExpression()
    {
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
        var arguments = ParseArguments(SyntaxKind.CloseParenthesisToken);
        var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
        return new CallExpressionSyntax(identifier, openParenthesisToken, arguments, closeParenthesisToken);
    }

    private SeparatedSyntaxList<ExpressionSyntax> ParseArguments(SyntaxKind endToken)
    {
        var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

        var parseNextArgument = true;
        while (parseNextArgument &&
               _lexemeQueue.Peek().Kind != endToken &&
               _lexemeQueue.Peek().Kind != SyntaxKind.EndToken)
        {
            var expression = ParseBinaryExpression();
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
        return new NameExpressionSyntax(identifierToken);
    }
}