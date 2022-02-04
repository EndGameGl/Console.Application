namespace Console.Application.Parsing.TypeParsers;

public class UInt16InputParser : GenericInputParser<ushort>
{
    public UInt16InputParser()
    {
        
    }

    protected override ushort ParseData(string input)
    {
        if (ushort.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}