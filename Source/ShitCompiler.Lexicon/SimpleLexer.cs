using System.Diagnostics;
using System.Text;
using ShitCompiler.Lexicon.Exceptions;

namespace ShitCompiler.Lexicon;

public class SimpleLexer: ILexer
{
    private TextCursor _textCursor;
    
    public SimpleLexer(TextCursor textCursor)
    {
        _textCursor = textCursor;
    }

    public Lexeme ScanNext()
    {   
        SkipWhiteSpaces();
        Location startingPosition = _textCursor.Location;
        LexemeKind kind = LexemeKind.Unknown;
        switch(_textCursor.PeekChar())
        {
            case '\"':
            case '\'':
                return this.ScanStringLiteral();
            case '/':
                _textCursor.Advance();
                if (_textCursor.TryAdvance('/') || _textCursor.TryAdvance('*'))
                {
                    _textCursor.Reset(startingPosition);
                    InvalidLexeme? lexeme = SkipComment();
                    return lexeme ?? ScanNext();
                }
                kind = LexemeKind.SlashToken;
                break;
            case '(':
                _textCursor.Advance();
                kind = LexemeKind.OpenParenToken;
                break;
            case ')':
                _textCursor.Advance();
                kind = LexemeKind.CloseParenToken;
                break;
            case '{':
                _textCursor.Advance();
                kind = LexemeKind.OpenBraceToken;
                break;
            case '}':
                _textCursor.Advance();
                kind = LexemeKind.CloseBraceToken;
                break;
            case '[':
                _textCursor.Advance();
                kind = LexemeKind.OpenBracketToken;
                break;
            case ']':
                _textCursor.Advance();
                kind = LexemeKind.CloseBracketToken;
                break;
            case ',':
                _textCursor.Advance();
                kind = LexemeKind.CommaToken;
                break;
            case '.':
                _textCursor.Advance();
                kind = LexemeKind.DotToken;
                break;
            case '+':
                _textCursor.Advance();
                kind = LexemeKind.PlusToken;
                break;
            case '-':
                _textCursor.Advance();
                kind = LexemeKind.MinusToken;
                break;
            case ':':
                _textCursor.Advance();
                kind = LexemeKind.ColonToken;
                break;
            case ';':
                _textCursor.Advance();
                kind = LexemeKind.SemicolonToken;
                break;
            case '*':
                _textCursor.Advance();
                kind = LexemeKind.AsteriskToken;
                break;
            case '>':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('=')
                    ? LexemeKind.GreaterThanEqualsToken 
                    : LexemeKind.GreaterThanToken;
                break;
            case '<':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('=')
                    ? LexemeKind.LessThanEqualsToken 
                    : LexemeKind.LessThanToken;
                break;
            case '=':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('=')
                    ? LexemeKind.EqualsEqualsToken
                    : LexemeKind.EqualsToken;
                break;
            case '!':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('=')
                    ? LexemeKind.ExclamationEqualsToken
                    : LexemeKind.Unknown;
                break;
            case '|':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('|')
                    ? LexemeKind.BarBarToken
                    : LexemeKind.Unknown;
                break;
            case '&':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('&')
                    ? LexemeKind.AmpersandAmpersandToken
                    : LexemeKind.Unknown;
                break;
            case TextCursor.InvalidCharacter:
                kind = LexemeKind.EndToken;
                break;
            
            case '_':
            case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z'):
                return ScanIdentifierOrKeyword();
            case >= '0' and <= '9':
                return ScanNumericLiteral();
            
            default:
                if (_textCursor.Advance() != 0)
                {
                    return new Lexeme(
                        LexemeKind.Unknown,
                        _textCursor.Slice(startingPosition, _textCursor.Location),
                        startingPosition
                    );
                }
                break;
        };
        
        return new Lexeme(
            kind,
            _textCursor.Slice(startingPosition, _textCursor.Location),
            startingPosition
        );
    }

    private Lexeme ScanNumericLiteral()
    {
        throw new NotImplementedException();
    }

    private Lexeme ScanIdentifierOrKeyword()
    {
        throw new NotImplementedException();
    }

    private InvalidLexeme? SkipComment()
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
                    
                    return new InvalidLexeme(
                        ErrorCode.IncompleteToken,
                        _textCursor.Slice(startingPosition, _textCursor.Location),
                        startingPosition
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
        while (char.IsWhiteSpace(_textCursor.PeekChar()))
            _textCursor.Advance();
    }

    private Lexeme ScanStringLiteral()
    {
        char quoteCharacter = _textCursor.PeekChar();
        Debug.Assert(quoteCharacter is ('\'' or '\"'));
        
        Location startingPosition = _textCursor.Location;
        LexemeKind kind = (_textCursor.PeekChar() == '\'')
            ? LexemeKind.StringLiteral
            : LexemeKind.CharacterLiteral;

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
                return new InvalidLexeme(
                    ErrorCode.IncompleteToken,
                    _textCursor.Slice(startingPosition, _textCursor.Location),
                    startingPosition
                );
            }
            ch = (ch != '\\')? ch : ScanEscapeSequence();
            
            builder.Append(ch);
        }
        ReadOnlyMemory<char> value = _textCursor.Slice(startingPosition, _textCursor.Location);
        
        //У символа строго ограниченная длина
        
        if (kind == LexemeKind.StringLiteral)
        {
            if (builder.Length == 1)
                return new Lexeme<char>(
                    LexemeKind.CharacterLiteral, 
                    value, 
                    startingPosition, 
                    builder[0]
                );
            
            return new InvalidLexeme(
                ErrorCode.TooManyCharactersInConstant,
                value,
                startingPosition
            );
        }

        return new Lexeme<string>(
            LexemeKind.StringLiteral,
            value,
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