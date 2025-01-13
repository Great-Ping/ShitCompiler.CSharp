using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax;

public enum SymbolTypes
{
    Void    = 0b000001,
    Array   = 0b000010,
    ULong    = 0b000100,
    Char    = 0b001000,
    String  = 0b010000, 
    Function= 0b100000
}

public record Symbol(
    Lexeme Identifier,    
    SymbolTypes Type
);

public record SymbolBlock(
    SymbolBlock? Parent,
    IDictionary<string, Symbol> Lexemes
)
{
    public Symbol? Find(string identifier, Location location)
    {
        if (!Lexemes.TryGetValue(identifier, out Symbol? value))
            return Parent?.Find(identifier, location);

        if (value.Identifier.Start.AbsoluteIndex > location.AbsoluteIndex)
            return Parent?.Find(identifier, location);

        return value;
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

    public void CreateNewSymbolBlock()
    {
        _current = new SymbolBlock(_current, new Dictionary<string, Symbol>());
    }

    public void DismissBlock()
    {
        _current = _current.Parent ?? _current;
    }
}
