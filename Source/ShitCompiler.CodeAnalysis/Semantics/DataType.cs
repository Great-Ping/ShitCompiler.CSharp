namespace ShitCompiler.CodeAnalysis.Semantics;

[Flags]
public enum DataType
{
    Unknown   = 0b000000000,
    Type      = 0b000000100,
    Unit      = 0b000001000,
    Boolean   = 0b000001000,
    Long      = 0b000010000,
    Double    = 0b000100000,
    Char      = 0b001000000,
    String    = 0b010000000,
    Array     = 0b100000000,
}