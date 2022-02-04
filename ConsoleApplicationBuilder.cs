using Console.Application.Parsing.TypeParsers;
using Console.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Application;

public class ConsoleApplicationBuilder
{
    internal List<Type> HostedServiceTypes { get; }
    public IServiceCollection Services { get; }
    internal IList<Type> TypeParsers { get; }

    internal ConsoleApplicationBuilder(string[] args)
    {
        HostedServiceTypes = new List<Type>();
        TypeParsers = new List<Type>();
        Services = new ServiceCollection();
        RegisterDefaultParsers();
    }

    private void RegisterDefaultParsers()
    {
        RegisterTypeParser<BooleanInputParser>();
        RegisterTypeParser<Int32InputParser>();
        RegisterTypeParser<StringInputParser>();
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