using Console.Application.Parsing.TypeParsers;
using Console.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Application;

public class ConsoleApplicationBuilder
{
    internal List<Type> HostedServiceTypes { get; }
    public IServiceCollection Services { get; }
    internal IList<Type> TypeParsers { get; }

    internal readonly string[] Args;

    internal ConsoleApplicationBuilder(string[] args)
    {
        Args = args;
        HostedServiceTypes = new List<Type>();
        TypeParsers = new List<Type>();
        Services = new ServiceCollection();
        RegisterDefaultParsers();
    }

    private void RegisterDefaultParsers()
    {
        RegisterTypeParser<BooleanInputParser>();

        RegisterTypeParser<Int16InputParser>();
        RegisterTypeParser<UInt16InputParser>();

        RegisterTypeParser<Int32InputParser>();
        RegisterTypeParser<UInt32InputParser>();

        RegisterTypeParser<Int64InputParser>();
        RegisterTypeParser<UInt64InputParser>();

        RegisterTypeParser<StringInputParser>();
        RegisterTypeParser<CharInputParser>();
        RegisterTypeParser<FloatInputParser>();
        RegisterTypeParser<DoubleInputParser>();
        RegisterTypeParser<DecimalInputParser>();
    }
    
    public ConsoleApplication Build()
    {
        return new ConsoleApplication(this);
    }

    public IServiceCollection AddHostedService<THostedServiceType>() where THostedServiceType : ConsoleApplicationHostedService
    {
        HostedServiceTypes.Add(typeof(THostedServiceType));
        return Services.AddSingleton<THostedServiceType>();
    }

    public IServiceCollection RegisterTypeParser<TTypeParser>() where TTypeParser : InputParser
    {
        TypeParsers.Add(typeof(TTypeParser));
        return Services.AddSingleton<TTypeParser>();
    }
}