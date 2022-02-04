namespace Console.Application.Parsing.TypeParsers;

public class DoubleInputParser : GenericInputParser<double>
{
    public DoubleInputParser()
    {
        
    }

    protected override double ParseData(string input)
    {
        if (double.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}