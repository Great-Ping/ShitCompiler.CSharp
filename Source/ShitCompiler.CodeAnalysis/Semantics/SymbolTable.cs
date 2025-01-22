using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Syntax;

public record SymbolBlock(
    SymbolBlock? Parent,
    IDictionary<string, Lexeme> Symbols
)
{
    public Lexeme? Find(Lexeme identifier)
    {
        return Find(identifier.OriginalValue, identifier.Start);
    }

    public Lexeme? FindInBlock(Lexeme identifier)
    {
        return FindInBlock(identifier.OriginalValue, identifier.Start);
    }

    public Lexeme? Find(string identifier, Location location)
    {
        if (!Symbols.TryGetValue(identifier, out Lexeme? value))
            return Parent?.Find(identifier, location);

        if (value.Start.AbsoluteIndex > location.AbsoluteIndex)
            return Parent?.Find(identifier, location);

        return value;
    }

    public Lexeme? FindInBlock(string identifier, Location location)
    {
        if (!Symbols.TryGetValue(identifier, out Lexeme? value))
            return null;
        
        if (value.Start.AbsoluteIndex > location.AbsoluteIndex)
            return null;

        return value;
    }

    public void AddSymbol(Lexeme lexeme)
    {
        Symbols.Add(lexeme.OriginalValue, lexeme);
    }

    public SymbolBlock CreateChild(){
        return new SymbolBlock(this, new Dictionary<string, Lexeme>());
    }
};

public class SymbolTable
{
    private SymbolBlock _current;

    public SymbolTable(){
        var root = new SymbolBlock(null, new Dictionary<string, Lexeme>());
        InitializeBaseTypes(root);
        _current = root;
    }

    private static void InitializeBaseTypes(SymbolBlock root)
    {
        root.AddSymbol(new Lexeme(
            SyntaxKind.IdentifierToken, 
            "long", 
            Location.Zero
        ));
        
        root.AddSymbol(new Lexeme(
            SyntaxKind.IdentifierToken,
            "double",
            Location.Zero
        ));
        
        root.AddSymbol(new Lexeme(
            SyntaxKind.IdentifierToken,
            "char",
            Location.Zero
        ));
        
        root.AddSymbol(new Lexeme(
            SyntaxKind.IdentifierToken,
            "string",
            Location.Zero
        ));
        
        root.AddSymbol(new Lexeme(
            SyntaxKind.IdentifierToken,
            "bool",
            Location.Zero
        ));
    }

    public SymbolBlock Current => _current;

    public Lexeme? Find(string identifier, Location location)
    {
        return _current.Find(identifier, location);
    }

    public Lexeme? Find(Lexeme lexeme)
    {
        if (lexeme.Kind != SyntaxKind.IdentifierToken)
            return null;

        return _current.Find(lexeme.OriginalValue, lexeme.Start);
    }

    public void AddSymbol(Lexeme lexeme)
    {
        _current.Symbols.TryAdd(lexeme.OriginalValue, lexeme);
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
