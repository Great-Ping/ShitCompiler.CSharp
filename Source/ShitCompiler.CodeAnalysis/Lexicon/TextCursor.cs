namespace ShitCompiler.CodeAnalysis.Lexicon;

public class TextCursor
{
    private ReadOnlyMemory<char> _text;
    private Location _location;
    
    /// <summary>
    /// char.MaxValue - одна из недопустимых значений для UTF-16
    /// </summary>
    public const char InvalidCharacter = char.MaxValue;
    public TextCursor(ReadOnlyMemory<char> inputText)
    {
        _text = inputText;
        _location = new Location(0,0,0);
    }
    
    public Location Location => _location;
    public bool HasMoreChars() => _location.AbsoluteIndex < _text.Length;

    public char NextChar()
    {
        Advance();
        return PeekChar();
    }

    public char PeekChar()
    {
        if (!HasMoreChars())
            return InvalidCharacter;
        
        return _text.Span[_location.AbsoluteIndex];
    }

    /// <summary>
    /// Text[point; this.Location) or Text[this.Location; point)  
    /// </summary>
    public ReadOnlyMemory<char> Slice(Location point)
    {
        if (point.AbsoluteIndex > _location.AbsoluteIndex)
            return Slice(Location, point);
        return Slice(point, Location);
    }
    
    /// <summary>
    /// Text[start; end)
    /// </summary>
    public ReadOnlyMemory<char> Slice(Location start, Location end)
    {
        return Slice(start, end.AbsoluteIndex - start.AbsoluteIndex);
    }
    
    public ReadOnlyMemory<char> Slice(Location start, int length)
    {
        return _text.Slice(start.AbsoluteIndex, length);
    }

    public void Reset(Location location)
    {
        _location = location;
    }

    public char PeekChar(int index)
    {
        int absoluteIndex = _location.AbsoluteIndex + index;
        
        if (absoluteIndex < 0 || absoluteIndex >= _text.Length)
            return InvalidCharacter;
        
        return _text.Span[absoluteIndex];
    }

    
    public bool TryAdvance(char character)
    {
        if (PeekChar() != character)
            return false;
        
        return Advance() > 0;
    }

    /// <summary>
    /// Продвижение на указаное количество символов, если возможно
    /// </summary>
    /// <param name="charCount">Количество символов для продвижения, по умолчанию 1</param>
    /// <returns>Количество пропущенных символов</returns>
    public int Advance(int charCount = 1)
    {
        int newPosition = _location.AbsoluteIndex + charCount;
        
        if (newPosition > _text.Length)
        {
            newPosition = _text.Length;
            charCount =  newPosition - _location.AbsoluteIndex;
        }
        
        DeterminateNewLocation(newPosition);
        return charCount;
    }

    private void DeterminateNewLocation(int newPosition)
    {
        Location location = _location;
        
        if (newPosition == location.AbsoluteIndex)
            return;
        
        int newSymbol = location.SymbolIndex;
        int newLine = location.LineIndex;

        for (int i = location.AbsoluteIndex; i < newPosition; i++)
        {
            if (_text.Span[i] != '\n')
            {
                newSymbol++;
                continue;
            }
            
            newSymbol = 0;
            newLine++;
        }

        _location = new Location(
            newPosition,
            newLine,
            newSymbol
        );
        
        
    }
    
    public override string ToString()
    {
        return new string(_text[_location.AbsoluteIndex..^1].Span);
    }
}