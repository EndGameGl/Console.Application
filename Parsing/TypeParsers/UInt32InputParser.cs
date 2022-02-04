namespace Console.Application.Parsing.TypeParsers;

public class UInt32InputParser : GenericInputParser<uint>
{
    public UInt32InputParser()
    {
    }

    protected override uint ParseData(string input)
    {
        if (uint.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}