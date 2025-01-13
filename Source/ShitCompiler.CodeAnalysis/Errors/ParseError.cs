namespace ShitCompiler.CodeAnalysis.Errors;

public class ParseError(Location location, string? message)
{
    public Location Location => location;
    public string? Message => message;
}