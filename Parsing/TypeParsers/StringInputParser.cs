namespace Console.Application.Parsing.TypeParsers;

internal class StringInputParser : GenericInputParser<string>
{
    public StringInputParser()
    {
    }

    protected override string ParseData(string text)
    {
        return text;
    }
}