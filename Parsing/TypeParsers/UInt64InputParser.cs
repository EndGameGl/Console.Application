namespace Console.Application.Parsing.TypeParsers;

public class UInt64InputParser : GenericInputParser<ulong>
{
    public UInt64InputParser()
    {
        
    }

    protected override ulong ParseData(string input)
    {
        if (ulong.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}