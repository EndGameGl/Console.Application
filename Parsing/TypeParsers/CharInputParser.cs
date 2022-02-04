namespace Console.Application.Parsing.TypeParsers;

internal class CharInputParser : GenericInputParser<char>
{
    public CharInputParser()
    {
        
    }

    protected override char ParseData(string input)
    {
        if (input is null || input.Length > 1)
        {
            throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
        }

        return input[0];
    }
}