namespace Console.Application.Parsing.TypeParsers;

public class DecimalInputParser : GenericInputParser<decimal>
{
    public DecimalInputParser()
    {
        
    }

    protected override decimal ParseData(string input)
    {
        if (decimal.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}