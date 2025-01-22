using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;

namespace ShitCompiler.CodeAnalysis.Semantics;

public class SymbolTable
{
    private readonly SymbolScope _root;
    private SymbolScope _current;

    public SymbolTable()
    {
        var root = new SymbolScope();
        InitializeBaseTypes(root);
        _root = root;
        _current = root;
    }

    private static void InitializeBaseTypes(SymbolScope root)
    {
        root.AddSymbol(new Symbol( 
            "unit", 
            0,
            DataType.Type,
            []
        ));

        root.AddSymbol(new Symbol( 
            "long", 
            0,
            DataType.Type,
            []
        ));
        
        root.AddSymbol(new Symbol(
            "double",
            0,
            DataType.Type,
            []
        ));
        
        root.AddSymbol(new Symbol(
            "char",
            0,
            DataType.Type,
            []
        ));
        
        root.AddSymbol(new Symbol(
            "string",
            0,
            DataType.Type,
            []
        ));
        
        root.AddSymbol(new Symbol(
            "bool",
            0,
            DataType.Type,
            []
        ));
    }


    public Symbol? Find(string identifier, int location)
    {
        return _current.Find(identifier, location);
    }

    public Symbol? Find(Lexeme lexeme)
    {
        if (lexeme.Kind != SyntaxKind.IdentifierToken)
            return null;

        return _current.Find(lexeme.OriginalValue, lexeme.Start.AbsoluteIndex);
    }

    public Symbol? FindInBlock(string identifier, int location)
    {
        return _current.FindInBlock(identifier, location);
    }

    public Symbol? FindInBlock(Lexeme lexeme)
    {
        return _current.FindInBlock(lexeme);
    }

    public void AddSymbol(Symbol lexeme)
    {
        _current.AddSymbol(lexeme);
    }

    public SymbolScope CreateNewSymbolBlock()
    {
        _current = _current.CreateChild();
        return _current;
    }

    public void DismissBlock()
    {
        _current = _current.Parent ?? _current;
    }

    public Symbol CreateTempSymbol(DataType dataType, int initialPosition, int[] arraySize, bool isFunk = false)
    {
        return _current.CreateTempSymbol(dataType, initialPosition, arraySize, isFunk);
    }


    public Symbol CreateTempSymbol(DataType dataType, int initialPosition, bool isFunk = false)
    {
        return _current.CreateTempSymbol(dataType, initialPosition, isFunk);
    }

    public void ResetBlock() 
    {
        _current = _root;
    }
}
