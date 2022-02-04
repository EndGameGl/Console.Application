namespace Console.Application.Routing.Attributes;

/// <summary>
///     Specifies options for command parameter
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class CommandOptionAttribute : Attribute
{
    /// <summary>
    ///     Alias under which this parameter would be called
    /// </summary>
    public string Alias { get; }

    /// <summary>
    ///     Whether this parameter is considered optional
    /// </summary>
    public bool IsOptional { get; }

    public CommandOptionAttribute(
        string alias = null,
        bool isOptional = false)
    {
        Alias = alias;
        IsOptional = isOptional;
    }
}