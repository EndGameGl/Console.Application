namespace Console.Application.Parsing.TypeParsers;

internal class BooleanInputParser : GenericInputParser<bool>
{
    public BooleanInputParser()
    {
        
    }
    
    protected override bool ParseData(string text)
    {
        if (bool.TryParse(text, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {typeof(bool).Name}: {text}");
    }
}