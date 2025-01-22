using System;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;

namespace ShitCompiler.CodeAnalysis.Semantics
{
    public record class Symbol(
        string Name,
        int InitLocation,
        DataType DataType,
        int[] ArraySize,
        bool IsFunk = false
    ) {
        bool IsArray => ArraySize.Length > 0;

        public Symbol(Lexeme lexeme, DataType dataType, bool isFunk = false)
            : this(lexeme, dataType, [], isFunk)
        { }

        public Symbol(
            Lexeme lexeme,
            DataType dataType,
            int[] arraySize, 
            bool isFunk = false
        ): this(
            lexeme.OriginalValue, 
            lexeme.Start.AbsoluteIndex,
            dataType, 
            arraySize,
            isFunk
        ) {
            
        }

        public Symbol(
            Lexeme lexeme,
            (DataType Type, int[] ArraySize) typeInfo, 
            bool isFunk = false
        ): this(
            lexeme,
            typeInfo.Type,
            typeInfo.ArraySize,
            isFunk
        ) {
            
        }
    };
}
