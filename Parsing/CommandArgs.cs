using System.Collections.ObjectModel;

namespace Console.Application.Parsing;

public class CommandArgs
{
    private static readonly ReadOnlyDictionary<string, string> EmptyDict = new(new Dictionary<string, string>(0));
    public string CommandText { get; }
    public ReadOnlyDictionary<string, string> Args { get; }
    public int ArgsCount => Args.Count;

    internal CommandArgs(
        string command,
        IDictionary<string, string>? values)
    {
        CommandText = command;
        Args = values is not null ? new ReadOnlyDictionary<string, string>(values) : EmptyDict;
    }
}