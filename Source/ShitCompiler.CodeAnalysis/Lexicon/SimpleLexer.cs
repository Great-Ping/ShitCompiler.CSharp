using System.Diagnostics;
using System.Globalization;
using System.Text;
using ShitCompiler.CodeAnalysis.Syntax;

namespace ShitCompiler.CodeAnalysis.Lexicon;

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
        SyntaxKind kind = SyntaxKind.BadToken;
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
                kind = SyntaxKind.OpenParenthesisToken;
                break;
            case ')':
                _textCursor.Advance();
                kind = SyntaxKind.CloseParenthesisToken;
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
                    
                    return new BadLexeme(
                        LexemeErrorCode.BadCharacterSequence,
                        _textCursor.SliceString(startingPosition),
                        startingPosition
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
                kind = SyntaxKind.StarToken;
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
                    : SyntaxKind.BadToken;
                break;
            case '|':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('|')
                    ? SyntaxKind.BarBarToken
                    : SyntaxKind.BadToken;
                break;
            case '&':
                _textCursor.Advance();
                kind = _textCursor.TryAdvance('&')
                    ? SyntaxKind.AmpersandAmpersandToken
                    : SyntaxKind.BadToken;
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
                    kind = SyntaxKind.BadToken; 
                    break;
                }
                kind = SyntaxKind.EndToken;
                break;
        };

        string originalValue = (kind == SyntaxKind.BadToken)
            ? _textCursor.SliceString(startingPosition)
            : SyntaxFacts.GetText(kind);

        return new Lexeme(
            kind,
            originalValue,
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

        string originalValue = _textCursor.SliceString(startingPosition);

        if (isReal)
        {
            if (double.TryParse(originalValue, CultureInfo.InvariantCulture, out double parsed))
            {
                return new Lexeme<double>(
                    SyntaxKind.RealNumberToken,
                    originalValue,
                    startingPosition,
                    parsed
                );
            }
        }
        else if (ulong.TryParse(originalValue, out ulong parsed))
        {
            return new Lexeme<long>(
                SyntaxKind.NumberToken,
                originalValue,
                startingPosition,
                (long)parsed
            );
        }


        return new BadLexeme(
            LexemeErrorCode.BadCharacterSequence,
            originalValue,
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

        SyntaxKind kind = SyntaxFacts.GetKeywordKind(
            text
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

    private BadLexeme? SkipComment()
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
                    
                    return new BadLexeme(
                        LexemeErrorCode.IncompleteToken,
                        _textCursor.SliceString(startingPosition),
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
        SyntaxKind kind = (_textCursor.PeekChar() == '\'')
            ? SyntaxKind.CharacterToken
            : SyntaxKind.StringToken;

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
                return new Lexeme(
                    SyntaxKind.BadToken,
                    _textCursor.SliceString(startingPosition),
                    startingPosition
                );
            }
            ch = (ch != '\\')? ch : ScanEscapeSequence();
            
            builder.Append(ch);
        }
        string originalValue = _textCursor.SliceString(startingPosition);
        
        //У символа строго ограниченная длина
        if (kind == SyntaxKind.CharacterToken)
        {
            if (builder.Length == 1)
                return new Lexeme<char>(
                    SyntaxKind.CharacterToken, 
                    originalValue, 
                    startingPosition, 
                    builder[0]
                );
            
            return new BadLexeme(
                LexemeErrorCode.TooManyCharactersInConstant,
                originalValue,
                startingPosition
            );
        }

        return new Lexeme<string>(
            SyntaxKind.StringToken,
            originalValue,
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