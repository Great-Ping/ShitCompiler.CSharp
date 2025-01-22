namespace ShitCompiler.CodeAnalysis.Semantics;

public class TypeInfo(
    DataType type,
    int[] arraySize
)
{
    public DataType Type => type;
    public int[] ArraySize => arraySize;
    
    public virtual bool Equals(TypeInfo? other)
    {
        if (other == null)
            return false;
        
        if (Type != other.Type)
            return false;
        
        if (ArraySize.Length != other.ArraySize.Length)
            return false;
    
        for (int i = 0; i < ArraySize.Length; i++)
        {   
            if (ArraySize[i] != other.ArraySize[i])
                return false;
        }
        
        return true;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine((int)Type, ArraySize);
    }

    public static implicit operator TypeInfo(DataType dataType)
        => new TypeInfo(dataType, []);
    
    public static implicit operator TypeInfo((DataType dataType, int[] arraySize) tuple)
        => new TypeInfo(tuple.dataType, tuple.arraySize);
    
    public static implicit operator DataType(TypeInfo dataType)
        => dataType.Type;

    public override string ToString()
    {
        if (!Type.HasFlag(DataType.Array))
            return Type.ToString();
        
        return $"{Type & (~DataType.Array)}[{string.Join(", ", ArraySize)}]";
    }
};

