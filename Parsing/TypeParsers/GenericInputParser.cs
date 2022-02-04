namespace Console.Application.Parsing.TypeParsers;

public abstract class GenericInputParser<TParsedType> : InputParser
{
    public override Type ParsedType { get; } = typeof(TParsedType);
    protected abstract TParsedType ParseData(string input);
    public override object Parse(string input)
    {
        return ParseData(input);
    }
}