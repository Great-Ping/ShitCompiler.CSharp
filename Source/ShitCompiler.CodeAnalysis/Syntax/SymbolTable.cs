using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax;

public enum SymbolTypes
{
    Void    = 0b000001,
    Array   = 0b000010,
    ULong    = 0b000100,
    Char    = 0b001000,
    String  = 0b010000, 
    Function= 0b100000,
    Unknown
}

public record Symbol
{
    public Lexeme Identifier { get; set; }
    public SymbolTypes Type { get; set; }

    public Symbol(Lexeme identifier, SymbolTypes type = SymbolTypes.Unknown)
    {
        Identifier = identifier;
        Type = type;
    }
};

public record SymbolBlock(
    SymbolBlock? Parent,
    IDictionary<string, Symbol> Symbols
)
{
    
    public Symbol? Find(Lexeme identifier)
    {
        return Find(identifier.OriginalValue, identifier.Start);
    }

    public Symbol? FindInBlock(Lexeme identifier)
    {
        return FindInBlock(identifier.OriginalValue, identifier.Start);
    }

    public Symbol? Find(string identifier, Location location)
    {
        if (!Symbols.TryGetValue(identifier, out Symbol? value))
            return Parent?.Find(identifier, location);

        if (value.Identifier.Start.AbsoluteIndex > location.AbsoluteIndex)
            return Parent?.Find(identifier, location);

        return value;
    }

    public Symbol? FindInBlock(string identifier, Location location)
    {
        if (!Symbols.TryGetValue(identifier, out Symbol? value))
            return null;
        
        if (value.Identifier.Start.AbsoluteIndex > location.AbsoluteIndex)
            return null;

        return value;
    }

    public void AddSymbol(Symbol symbol)
    {
        Symbols.Add(symbol.Identifier.OriginalValue, symbol);
    }

    public SymbolBlock CreateChild(){
        return new SymbolBlock(this, new Dictionary<string, Symbol>());
    }
};

public class SymbolTable
{
    private SymbolBlock _current;

    public SymbolTable(){
        var root = new SymbolBlock(null, new Dictionary<string, Symbol>());
        _current = root;
    }
    
    public SymbolBlock Current => _current;

    public Symbol? Find(string identifier, Location location)
    {
        return _current.Find(identifier, location);
    }

    public Symbol? Find(Lexeme lexeme)
    {
        if (lexeme.Kind != SyntaxKind.IdentifierToken)
            return null;

        return _current.Find(lexeme.OriginalValue, lexeme.Start);
    }

    public void AddSymbol(Symbol symbol)
    {
        _current.Symbols.Add(symbol.Identifier.OriginalValue, symbol);
    }

    public SymbolBlock CreateNewSymbolBlock()
    {
        _current = _current.CreateChild();
        return _current;
    }

    public void DismissBlock()
    {
        _current = _current.Parent ?? _current;
    }
}
