namespace ShitCompiler.CodeAnalysis.Semantics;

public class TypeInfo(
    DataType type,
    int[] arraySize
)
{
    public DataType Type { get; set; } = type;
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
    
    public static bool operator==(TypeInfo? arg1, TypeInfo? arg2)
    {
        if (arg1 is null || arg2 is null)
            return false;
        
        return arg1.Equals(arg2);
    }

    public static bool operator !=(TypeInfo arg1, TypeInfo arg2)
    {
        return !(arg1 == arg2);
    }

    public static implicit operator TypeInfo(DataType dataType)
        => new TypeInfo(dataType, []);
    
    public static implicit operator TypeInfo((DataType dataType, int[] arraySize) tuple)
        => new TypeInfo(tuple.dataType, tuple.arraySize);

    public override string ToString()
    {
        if (!Type.HasFlag(DataType.Array))
            return Type.ToString();
        
        return $"{Type & (~DataType.Array)}[{string.Join(", ", ArraySize)}]";
    }
};

