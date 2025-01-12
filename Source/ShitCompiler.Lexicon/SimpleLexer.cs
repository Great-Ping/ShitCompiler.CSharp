using System.Diagnostics;
using System.Globalization;
using System.Text;
using ShitCompiler.Lexicon.Exceptions;

namespace ShitCompiler.Lexicon;

public class SimpleLexer: ILexer
{
    private TextCursor _textCursor;
    private Dictionary<string, LexemeKind> _keywords = new(){
        { "if" ,  LexemeKind.IfKeyword },
        { "else" ,  LexemeKind.ElseKeyword },
        { "var", LexemeKind.VarKeyword },
        { "val" ,  LexemeKind.ValKeyword },
    };
    
    
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
                return ScanStringLiteral();
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
                if (char.IsDigit(_textCursor.PeekChar()))
                {
                    while (char.IsDigit(_textCursor.PeekChar()))
                        _textCursor.Advance();
                    
                    return new InvalidLexeme(
                        ErrorCode.UnknownCharactersSequence, 
                        _textCursor.Slice(startingPosition), 
                        startingPosition
                    );
                }
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
                        _textCursor.Slice(startingPosition),
                        startingPosition
                    );
                }
                break;
        };
        
        return new Lexeme(
            kind,
            _textCursor.Slice(startingPosition),
            startingPosition
        );
    }

    private Lexeme ScanNumericLiteral()
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

        ReadOnlyMemory<char> value = _textCursor.Slice(startingPosition);

        if (isReal)
        {
            if (double.TryParse(value.Span, CultureInfo.InvariantCulture, out double parsed))
            {
                return new Lexeme<double>(
                    LexemeKind.RealNumberToken,
                    value,
                    startingPosition,
                    parsed
                );
            }
        }
        else if (ulong.TryParse(value.Span, out ulong parsed))
        {
            return new Lexeme<ulong>(
                LexemeKind.NumberToken,
                value,
                startingPosition,
                parsed
            );
        }


        return new InvalidLexeme(
            ErrorCode.UnknownCharactersSequence,
            _textCursor.Slice(startingPosition),
            startingPosition
        );
    }
    
    private Lexeme ScanIdentifierOrKeyword()
    {
        Location starting = _textCursor.Location;
        char character = _textCursor.PeekChar();
        
        while (IsIdentifierCharacter(character))
            character = _textCursor.NextChar();   
        
        string text = _textCursor.Slice(starting).ToString();
        
        LexemeKind kind = _keywords.GetValueOrDefault(
            text,
            LexemeKind.IdentifierToken
        );
        
        return new Lexeme(
            kind,
            text.AsMemory(),
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
        char character = _textCursor.PeekChar();
        while (char.IsWhiteSpace(character) || character == '\n' || character == '\r' || character == '\t')
            character = _textCursor.NextChar();

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
                    _textCursor.Slice(startingPosition),
                    startingPosition
                );
            }
            ch = (ch != '\\')? ch : ScanEscapeSequence();
            
            builder.Append(ch);
        }
        ReadOnlyMemory<char> value = _textCursor.Slice(startingPosition);
        
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