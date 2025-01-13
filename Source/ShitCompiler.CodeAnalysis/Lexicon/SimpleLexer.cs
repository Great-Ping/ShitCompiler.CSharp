using System.Diagnostics;
using System.Globalization;
using System.Text;
using ShitCompiler.CodeAnalysis.Lexicon.Errors;
using ShitCompiler.CodeAnalysis.Syntax;

namespace ShitCompiler.CodeAnalysis.Lexicon;

public class SimpleLexer: ILexer
{
    private TextCursor _textCursor;
    private Dictionary<string, SyntaxKind> _keywords = new(){
        { "if" ,  SyntaxKind.IfKeyword },
        { "else" ,  SyntaxKind.ElseKeyword },
        { "var", SyntaxKind.VarKeyword },
        { "val" ,  SyntaxKind.ValKeyword },
    };
    
    
    public SimpleLexer(TextCursor textCursor)
    {
        _textCursor = textCursor;
    }

    public ParseResult<Lexeme> ScanNext()
    {   
        SkipWhiteSpaces();
        Location startingPosition = _textCursor.Location;
        SyntaxKind kind = SyntaxKind.Unknown;
        switch(_textCursor.PeekChar())
        {
            case '\"':
            case '\'':
                return ScanStringLiteral();
            case '/':
                _textCursor.Advance();
                if (_textCursor.TryAdvance('/') || _textCursor.TryAdvance('*'))
                {
                    _textCursor.Reset(startingPosition);
                    //Игнорируются только правильные комментарии,
                    //если в комментарии ошибка, то она всплывает
                    return SkipComment() ?? ScanNext();
                }
                kind = SyntaxKind.SlashToken;
                break;
            case '(':
                _textCursor.Advance();
                kind = SyntaxKind.OpenParenToken;
                break;
            case ')':
                _textCursor.Advance();
                kind = SyntaxKind.CloseParenToken;
                break;
            case '{':
                _textCursor.Advance();
                kind = SyntaxKind.OpenBraceToken;
                break;
            case '}':
                _textCursor.Advance();
                kind = SyntaxKind.CloseBraceToken;
                break;
            case '[':
                _textCursor.Advance();
                kind = SyntaxKind.OpenBracketToken;
                break;
            case ']':
                _textCursor.Advance();
                kind = SyntaxKind.CloseBracketToken;
                break;
            case ',':
                _textCursor.Advance();
                kind = SyntaxKind.CommaToken;
                break;
            case '.':
                _textCursor.Advance();
                kind = SyntaxKind.DotToken;
                if (char.IsDigit(_textCursor.PeekChar()))
                {
                    while (char.IsDigit(_textCursor.PeekChar()))
                        _textCursor.Advance();
                    
                    return new BadCharactersSquenceError(
                        startingPosition,
                        new string(_textCursor.Slice(startingPosition).Span)
                    );
                }
                break;
            case '+':
                _textCursor.Advance();
                kind = SyntaxKind.PlusToken;
                break;
            case '-':
                _textCursor.Advance();
                kind = SyntaxKind.MinusToken;
                break;
            case ':':
                _textCursor.Advance();
                kind = SyntaxKind.ColonToken;
                break;
            case ';':
                _textCursor.Advance();
                kind = SyntaxKind.SemicolonToken;
                break;
            case '*':
                _textCursor.Advance();
                kind = SyntaxKind.AsteriskToken;
                break;
            case '>':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('=')
                    ? SyntaxKind.GreaterThanEqualsToken 
                    : SyntaxKind.GreaterThanToken;
                break;
            case '<':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('=')
                    ? SyntaxKind.LessThanEqualsToken 
                    : SyntaxKind.LessThanToken;
                break;
            case '=':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('=')
                    ? SyntaxKind.EqualsEqualsToken
                    : SyntaxKind.EqualsToken;
                break;
            case '!':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('=')
                    ? SyntaxKind.ExclamationEqualsToken
                    : SyntaxKind.Unknown;
                break;
            case '|':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('|')
                    ? SyntaxKind.BarBarToken
                    : SyntaxKind.Unknown;
                break;
            case '&':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('&')
                    ? SyntaxKind.AmpersandAmpersandToken
                    : SyntaxKind.Unknown;
                break;
            case TextCursor.InvalidCharacter:
                kind = SyntaxKind.EndToken;
                break;
            
            case '_' or (>= 'a' and <= 'z') or (>= 'A' and <= 'Z'):
                return ScanIdentifierOrKeyword();
            case >= '0' and <= '9':
                return ScanNumericLiteral();
            
            default:
                if (_textCursor.Advance() != 0)
                {
                    kind = SyntaxKind.Unknown; 
                    break;
                }
                kind = SyntaxKind.EndToken;
                break;
        };
        return new Lexeme(
            kind,
            new string(_textCursor.Slice(startingPosition).Span),
            startingPosition
        );
    }

    private ParseResult<Lexeme> ScanNumericLiteral()
    {
        bool isReal = false;
        Location startingPosition = _textCursor.Location;
        char character = _textCursor.PeekChar();

        
        while (char.IsDigit(character) || character == '.')
        {
            if (character == '.')
                isReal = true;
            character = _textCursor.NextChar();
        }

        ReadOnlySpan<char> value = _textCursor.Slice(startingPosition).Span;

        if (isReal)
        {
            if (double.TryParse(value, CultureInfo.InvariantCulture, out double parsed))
            {
                return new Lexeme<double>(
                    SyntaxKind.RealNumberToken,
                    new string(value),
                    startingPosition,
                    parsed
                );
            }
        }
        else if (ulong.TryParse(value, out ulong parsed))
        {
            return new Lexeme<ulong>(
                SyntaxKind.NumberToken,
                new string(value),
                startingPosition,
                parsed
            );
        }


        return new BadCharactersSquenceError(
            startingPosition,
            new string(value)
        );
    }
    
    private Lexeme ScanIdentifierOrKeyword()
    {
        Location starting = _textCursor.Location;
        char character = _textCursor.PeekChar();
        
        while (IsIdentifierCharacter(character))
            character = _textCursor.NextChar();   
        
        string text = _textCursor.Slice(starting).ToString();
        
        SyntaxKind kind = _keywords.GetValueOrDefault(
            text,
            SyntaxKind.IdentifierToken
        );
        
        return new Lexeme(
            kind,
            text,
            starting
        );
    }

    private bool IsIdentifierCharacter(char character)
    {
        return character 
            is (>= 'a' and <= 'z') 
            or (>= 'A' and <= 'Z') 
            or (>= '0' and <= '9' or '_');
    }

    private IncompleteTokenError? SkipComment()
    {
        Location startingPosition = _textCursor.Location;
        char slashCharacter = _textCursor.PeekChar();
        Debug.Assert(slashCharacter == '/');
        char resolveCharacter = _textCursor.NextChar();
        Debug.Assert(resolveCharacter is ('/' or '*'));
        
        switch (resolveCharacter)
        {
            case '*':
                while (!(_textCursor.TryAdvance('*') && _textCursor.TryAdvance('/')))
                {
                    if (_textCursor.Advance() != 0)
                        continue;
                    
                    ReadOnlySpan<char> value = _textCursor.Slice(startingPosition).ToString();
                    return new IncompleteTokenError(
                        startingPosition,
                        badToken: new Lexeme(
                            SyntaxKind.CommentTrivia,
                            new string(value),
                            startingPosition
                        )
                    );
                }
                break;
            
            case '/':
                while (!_textCursor.TryAdvance('\n') && _textCursor.Advance() != 0)
                { }
                break;
        }
        return null;
    }
    
    private void SkipWhiteSpaces()
    {
        char character = _textCursor.PeekChar();
        while (char.IsWhiteSpace(character) || character == '\n' || character == '\r' || character == '\t')
            character = _textCursor.NextChar();

    }

    private ParseResult<Lexeme> ScanStringLiteral()
    {
        char quoteCharacter = _textCursor.PeekChar();
        Debug.Assert(quoteCharacter is ('\'' or '\"'));
        
        Location startingPosition = _textCursor.Location;
        SyntaxKind kind = (_textCursor.PeekChar() == '\'')
            ? SyntaxKind.StringLiteral
            : SyntaxKind.CharacterLiteral;

        StringBuilder builder = new();
        
        while (true)
        {
            char ch = _textCursor.NextChar();

            if (ch == quoteCharacter)
            {
                _textCursor.Advance();
                break;
            }
            
            if (ch == TextCursor.InvalidCharacter)
            {
                ReadOnlySpan<char> originalValue = _textCursor.Slice(startingPosition).Span;
                return new IncompleteTokenError(
                    startingPosition,
                    badToken: new Lexeme(
                        SyntaxKind.StringLiteral,
                        new string(originalValue),
                        startingPosition
                    )
                );
            }
            ch = (ch != '\\')? ch : ScanEscapeSequence();
            
            builder.Append(ch);
        }
        ReadOnlyMemory<char> value = _textCursor.Slice(startingPosition);
        
        
        //У символа строго ограниченная длина
        if (kind == SyntaxKind.CharacterLiteral)
        {
            if (builder.Length == 1)
                return new Lexeme<char>(
                    SyntaxKind.CharacterLiteral, 
                    new string(value.Span), 
                    startingPosition, 
                    builder[0]
                );
            
            return new TooManyCharactersInConstant(
                startingPosition,
                badToken: new Lexeme(
                    SyntaxKind.CharacterLiteral,
                    new string(value.Span),
                    startingPosition
                )
            );
        }

        return new Lexeme<string>(
            SyntaxKind.StringLiteral,
            new string(value.Span),
            startingPosition,
            builder.ToString()
        );
    }
    
    private char ScanEscapeSequence()
    {
        char slashCharacter = _textCursor.NextChar();
        Debug.Assert(slashCharacter == '\\');
        
        char ch = _textCursor.NextChar();
        switch (_textCursor.NextChar())
        {
            case '\'':
            case '"':
            case '\\':
                break;
            case '0':
                ch = '\u0000';
                break;
            case 'a':
                ch = '\u0007';
                break;
            case 'b':
                ch = '\u0008';
                break;
            case 'e':
                ch = '\u001b';
                break;
            case 'f':
                ch = '\u000c';
                break;
            case 'n':
                ch = '\u000a';
                break;
            case 'r':
                ch = '\u000d';
                break;
            case 't':
                ch = '\u0009';
                break;
            case 'v':
                ch = '\u000b';
                break;
            case 'x':
            case 'u':
            case 'U':
            default:
                return TextCursor.InvalidCharacter;
        }

        return ch;
    }
}