namespace Console.Application.Routing;

public class CommandHandlerBase
{
    protected void Write(string text)
    {
        System.Console.WriteLine(text);
    }

    protected void Write(string text, ConsoleColor textColor)
    {
        var colorBefore = System.Console.ForegroundColor;
        System.Console.ForegroundColor = textColor;
        System.Console.WriteLine(text);
        System.Console.ForegroundColor = colorBefore;
    }
}