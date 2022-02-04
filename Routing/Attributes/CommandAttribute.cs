using System.Runtime.CompilerServices;

namespace Console.Application.Routing.Attributes;

/// <summary>
///     Specifies commands name / description
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    /// <summary>
    ///     Command name
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    ///     Command description
    /// </summary>
    public string Description { get; }

    public CommandAttribute(
        [CallerMemberName] string methodName = null,
        string description = null)
    {
        MethodName = methodName;
        Description = description;
    }
}