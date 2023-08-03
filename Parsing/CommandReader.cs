using System.Text;

namespace Console.Application.Parsing;

internal class CommandReader
{
    private readonly CommandHandler _handler;

    internal CommandReader(CommandHandler handler)
    {
        _handler = handler;
    }

    public void ReadCommand()
    {
        var currentInput = "";
        
        while (true)
        {
            var nextKey = System.Console.ReadKey();
            if (nextKey.Key is ConsoleKey.Backspace)
            {
                currentInput = currentInput[..^1];
            }
            
            var options = _handler.GetPossibleExecutors(currentInput);        
        }
    }
}