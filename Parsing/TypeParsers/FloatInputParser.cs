namespace Console.Application.Parsing.TypeParsers;

internal class FloatInputParser : GenericInputParser<float>
{
    public FloatInputParser()
    {
    }

    protected override float ParseData(string input)
    {
        if (float.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}