using System.Diagnostics.CodeAnalysis;
using ShitCompiler.CodeAnalysis.Syntax.Errors;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis;

public readonly struct ParseResult<TValue>
    where TValue : ISyntaxNode
{
    private readonly TValue? _value;
    private readonly ParseError? _error;
    private readonly bool _isSuccess;
    
    private ParseResult(TValue? value, ParseError? error, bool isSuccess)
    {
        _value = value;
        _error = error;
        _isSuccess = isSuccess;
    }
    
    
    public bool TrySuccess(
        [MaybeNullWhen(false)] out TValue value
    ){
        value = _value;
        return _isSuccess;
    }
    
    public bool TrySuccess(
        [MaybeNullWhen(false)] out TValue value, 
        [MaybeNullWhen(true)] out ParseError error){
        value = _value;
        error = _error;
        return _isSuccess;
    }

    public bool TryFailure(
        [MaybeNullWhen(false)] out ParseError error
    ){
        error = _error;
        return !_isSuccess;
    }
    
    public bool TryFailure(
        [MaybeNullWhen(true)] out TValue value, 
        [MaybeNullWhen(false)] out ParseError error){
        value = _value;
        error = _error;
        return !_isSuccess;
    }
    
    public static ParseResult<TValue> Ok(TValue success)
        => new(success, default, true);
    
    public static ParseResult<TValue> Fail(ParseError error)
        => new(default, error, false);
    
    public static implicit operator ParseResult<TValue>(TValue success)
        => Ok(success);

    public static implicit operator ParseResult<TValue>(ParseError error)
        => Fail(error);

    public override string ToString()
    {
        return TrySuccess(out var value, out var error)
            ? $"Success({value})" 
            : $"Error({error})";
    }
};
