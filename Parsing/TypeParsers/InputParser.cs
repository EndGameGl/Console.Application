namespace Console.Application.Parsing.TypeParsers;

public abstract class InputParser
{
    public abstract Type ParsedType { get; }
    public abstract object Parse(string input);
}