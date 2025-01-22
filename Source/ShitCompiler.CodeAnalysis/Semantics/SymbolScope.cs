using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;

namespace ShitCompiler.CodeAnalysis.Semantics
{
    public class SymbolScope(
        SymbolScope? parent,
        IDictionary<string, Symbol> symbols,
        string id
    ) {
        private int _temporarySymbolsCount = 0;
        private int _childsCount = 0;
        private SymbolScope? _parent = parent;
        private IDictionary<string, Symbol> _symbols = symbols;
        private string _id = id;

        public SymbolScope(SymbolScope? parent = null, string id = "₽")
            :this(parent, new Dictionary<string, Symbol>(), id)
        { }

        public string Id => _id;
        public SymbolScope? Parent => _parent;

        public Symbol? Find(Lexeme identifier)
        {
            return Find(identifier.OriginalValue, identifier.Start.AbsoluteIndex);
        }

        public Symbol? FindInBlock(Lexeme identifier)
        {
            return FindInBlock(identifier.OriginalValue, identifier.Start.AbsoluteIndex);
        }

        public Symbol? Find(string identifier, int position)
        {
            if (!_symbols.TryGetValue(identifier, out Symbol? value))
                return _parent?.Find(identifier, position);

            if (value.InitLocation > position)
                return _parent?.Find(identifier, position);

            return value;
        }

        public Symbol? FindInBlock(string identifier, int location)
        {
            if (!_symbols.TryGetValue(identifier, out Symbol? value))
                return null;

            if (value.InitLocation > location)
                return null;

            return value;
        }

        public void AddSymbol(Symbol lexeme)
        {
            _symbols.TryAdd(lexeme.Name, lexeme);
        }

        public SymbolScope CreateChild()
        {
            _childsCount++;
            return new SymbolScope(this, id + $"-c{_childsCount}");
        }

        public Symbol CreateTempSymbol(DataType dataType, int initLocation, bool isFunk = false)
        {
            return CreateTempSymbol(dataType, initLocation, [], isFunk);
        }

        public Symbol CreateTempSymbol(DataType dataType, int initLocation, int[] arraySize, bool isFunk = false)
        {
            _temporarySymbolsCount++;
            Symbol temp = new Symbol(
                _id + $"{_temporarySymbolsCount}",
                initLocation,
                dataType,
                arraySize,
                isFunk                
            );
            AddSymbol(temp);
            return temp;
        }
    };

}
