namespace Console.Application.Routing.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandRouteAttribute : Attribute
{
    public string Route { get; }

    public CommandRouteAttribute(string route)
    {
        Route = route;
    }
}