namespace Console.Application.Parsing.TypeParsers;

public class Int64InputParser : GenericInputParser<long>
{
    public Int64InputParser()
    {
        
    }

    protected override long ParseData(string input)
    {
        if (long.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}