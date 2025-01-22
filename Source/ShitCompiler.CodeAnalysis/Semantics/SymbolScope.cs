using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShitCompiler.CodeAnalysis.Lexicon;

namespace ShitCompiler.CodeAnalysis.Semantics
{
    public record SymbolScope(
        SymbolScope? Parent,
        IDictionary<string, Symbol> Symbols
    ){
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
            if (!Symbols.TryGetValue(identifier, out Symbol? value))
                return Parent?.Find(identifier, position);

            if (value.InitLocation > position)
                return Parent?.Find(identifier, position);

            return value;
        }

        public Symbol? FindInBlock(string identifier, int location)
        {
            if (!Symbols.TryGetValue(identifier, out Symbol? value))
                return null;

            if (value.InitLocation > location)
                return null;

            return value;
        }

        public void AddSymbol(Symbol lexeme)
        {
            Symbols.TryAdd(lexeme.Name, lexeme);
        }

        public SymbolScope CreateChild()
        {
            return new SymbolScope(this, new Dictionary<string, Symbol>());
        }
    };

}
