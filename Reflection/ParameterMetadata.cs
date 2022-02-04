namespace Console.Application.Reflection;

public class ParameterMetadata
{
    public int Order { get; }
    public string Name { get; }
    public bool CanBeNull { get; }
    public bool IsOptional { get; }
    public Type DataType { get; }

    public ParameterMetadata(int order, string name, bool canBeBull, bool isOptional, Type dataType)
    {
        Order = order;
        Name = name;
        CanBeNull = canBeBull;
        IsOptional = isOptional;
        DataType = dataType;
    }
}