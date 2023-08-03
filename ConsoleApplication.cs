using System.Collections.ObjectModel;
using System.Reflection;
using Console.Application.Parsing;
using Console.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Application;

public class ConsoleApplication
{
    private IReadOnlyCollection<Type> HostedServiceTypes { get; }
    private ParameterParser ParameterParser { get; }
    private CommandParser CommandParser { get; }
    private CommandHandler CommandHandler { get; }
    public IServiceProvider ServiceProvider { get; }

    private readonly string[] _args;

    public static ConsoleApplicationBuilder CreateBuilder(string[] args)
    {
        return new ConsoleApplicationBuilder(args);
    }

    internal ConsoleApplication(
        ConsoleApplicationBuilder builder)
    {
        _args = builder.Args;
        CommandParser = new CommandParser(new[] { "-" });
        CommandHandler = new CommandHandler(builder.Services);
        ServiceProvider = builder.Services.BuildServiceProvider();
        HostedServiceTypes = new ReadOnlyCollection<Type>(builder.HostedServiceTypes);
        ParameterParser = new ParameterParser(ServiceProvider, builder.TypeParsers);
    }

    public async Task RunAsync()
    {
        System.Console.WriteLine("Starting application...");

        foreach (var type in HostedServiceTypes)
        {
            var service = (ConsoleApplicationHostedService)ServiceProvider.GetRequiredService(type);
            await service.OnStart();
        }

        if (_args.Length > 0)
        {          
            var startupCommand = string.Join(" ", _args.Select(x =>
            {
                if (x.Contains(' '))
                    return $"\"{x}\"";
                return x;
            }));
            var commandArgs = CommandParser.ParseCommand(startupCommand);
            await CommandHandler.HandleCommandLine(commandArgs, ServiceProvider, ParameterParser);
            await OnExit();
            return;
        }

        var shouldExit = false;
        System.Console.WriteLine("Waiting for input! Write [help] to see all commands or [exit] to exit this app.");

        while (!shouldExit)
        {
            System.Console.Write("> ");
            var nextLine = System.Console.ReadLine();
            switch (nextLine)
            {
                case "help":
                    CommandHandler.HandleHelpCall();
                    break;
                case "exit":
                    await OnExit();
                    shouldExit = true;
                    break;
                default:
                    var commandArgs = CommandParser.ParseCommand(nextLine);
                    await CommandHandler.HandleCommandLine(commandArgs, ServiceProvider, ParameterParser);
                    break;
            }
        }

        System.Console.WriteLine("Closing application...");
    }

    internal async Task OnExit()
    {
        foreach (var type in HostedServiceTypes)
        {
            var service = (ConsoleApplicationHostedService)ServiceProvider.GetRequiredService(type);
            await service.OnExit();
        }

        await (ServiceProvider as ServiceProvider).DisposeAsync();
    }
}