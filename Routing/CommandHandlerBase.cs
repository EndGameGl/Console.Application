using Console.Application.Routing.Attributes;

namespace Console.Application.Routing;


public class CommandHandlerBase
{
    protected void Write(string text)
    {
        System.Console.WriteLine(text);
    }
}