using Console.Application.Parsing;
using Console.Application.Reflection;
using Console.Application.Routing;
using Console.Application.Routing.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Console.Application;

internal class CommandHandler
{
    private readonly Type _commandHandleBaseType = typeof(CommandHandlerBase);

    private readonly ReadOnlyDictionary<string, CommandHandlerExecutor> _commandHandlerExecutors;

    internal CommandHandler(IServiceCollection serviceCollection)
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly is null)
            throw new Exception("Failed to gather assembly data");
        var commandHandlers = FindAvailableCommandHandlers(assembly);

        var handlerExecutors = new Dictionary<string, CommandHandlerExecutor>();

        foreach (var commandHandler in commandHandlers)
        {
            serviceCollection.AddTransient(commandHandler);
            var routeAttribute = commandHandler.GetCustomAttribute<CommandRouteAttribute>();
            var baseRoute = routeAttribute.Route;

            foreach (var methodInfo in commandHandler.DeclaredMethods)
            {
                var commandAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute is null)
                    continue;

                var methodAlias = commandAttribute.MethodName;
                var finalRoute = string.Join(' ', baseRoute, methodAlias);
                var executor = new CommandHandlerExecutor(
                    finalRoute,
                    commandAttribute.Description,
                    commandHandler,
                    methodInfo);
                if (!handlerExecutors.TryAdd(finalRoute, executor))
                {
                    throw new Exception(
                        $"Error when adding command handler for: {commandHandler.Name}.{methodInfo.Name}");
                }
            }
        }

        _commandHandlerExecutors = new ReadOnlyDictionary<string, CommandHandlerExecutor>(handlerExecutors);
    }

    internal async Task HandleCommandLine(
        CommandArgs commandArgs,
        IServiceProvider serviceProvider,
        ParameterParser parameterParser)
    {
        if (_commandHandlerExecutors.TryGetValue(commandArgs.CommandText, out var executor))
        {
            if (executor.IsAsync)
            {
                await executor.ExecuteAsync(serviceProvider, commandArgs, parameterParser);
            }
            else
            {
                executor.Execute(serviceProvider, commandArgs, parameterParser);
            }
        }
    }

    internal void HandleHelpCall()
    {
        var sb = new StringBuilder();
        sb.AppendLine("List of available commands: ");
        sb.AppendLine("exit - exits the app");
        sb.AppendLine("help - lists available commands");

        foreach (var (_, executor) in _commandHandlerExecutors)
        {
            sb.AppendLine($"{executor.Path} - {executor.Description}");
            var parameters = executor.ParameterMetadatas;
            if (!parameters.TryGetNonEnumeratedCount(out var paramCount))
                paramCount = parameters.Count();
            if (paramCount > 0)
            {
                foreach (var parameter in parameters)
                {
                    sb.AppendLine($"    {parameter.Order}: {parameter.Name} - {parameter.DataType.Name}");
                }
            }
        }

        System.Console.WriteLine(sb.ToString());
    }

    internal IEnumerable<TypeInfo> FindAvailableCommandHandlers(Assembly assembly)
    {
        return assembly.DefinedTypes.Where(x => x.IsAssignableTo(_commandHandleBaseType));
    }

    internal CommandHandlerExecutor[] GetPossibleExecutors(string commandNameInput)
    {
        return _commandHandlerExecutors.Where(x => x.Key.Contains(commandNameInput)).Select(x => x.Value).ToArray();
    }
}