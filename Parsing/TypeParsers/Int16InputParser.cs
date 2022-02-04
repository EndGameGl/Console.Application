namespace Console.Application.Parsing.TypeParsers;

public class Int16InputParser : GenericInputParser<short>
{
    public Int16InputParser()
    {
        
    }

    protected override short ParseData(string input)
    {
        if (short.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}