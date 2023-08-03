using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Console.Application.Parsing;
using Console.Application.Routing;
using Console.Application.Routing.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Application.Reflection;

/// <summary>
///     Class for handling execution process of commands
/// </summary>
internal class CommandHandlerExecutor
{
    private static Type NullableType { get; } = typeof(Nullable<>);

    private readonly string _description;
    private readonly string _path;
    private readonly Type _handlerType;
    private readonly MethodInfo _methodToExecute;
    private ReadOnlyDictionary<string, ParameterMetadata> _parameterMetadatas;

    /// <summary>
    ///     Whether this operation should be executed async
    /// </summary>
    internal bool IsAsync { get; private set; }

    /// <summary>
    ///     Path of this command
    /// </summary>
    internal string Path => _path;

    /// <summary>
    ///     Description of this command
    /// </summary>
    internal string Description => _description;

    /// <summary>
    /// Metadata for all existing parameters in this method
    /// </summary>
    internal IEnumerable<ParameterMetadata> ParameterMetadatas => _parameterMetadatas.Values;

    /// <summary>
    ///     Default .ctor for creating Command Executor
    /// </summary>
    /// <param name="path">Path to this command</param>
    /// <param name="description">Description of this command</param>
    /// <param name="handlerType">Command handler which will execute this command</param>
    /// <param name="methodToExecute">Method with handler should execute</param>
    internal CommandHandlerExecutor(
        string path,
        string description,
        Type handlerType,
        MethodInfo methodToExecute)
    {
        _path = path;
        _description = description;
        _handlerType = handlerType;
        _methodToExecute = methodToExecute;
        ConstructParametersInfo();
        CheckIfAsync();
    }

    /// <summary>
    ///     Determines whether this method is async
    /// </summary>
    private void CheckIfAsync()
    {
        IsAsync = _methodToExecute.GetCustomAttribute<AsyncStateMachineAttribute>() is not null;
    }

    /// <summary>
    ///     Constructs parameter metadata for this command
    /// </summary>
    private void ConstructParametersInfo()
    {
        var parameters = _methodToExecute.GetParameters();
        var parameterMetadatas = new Dictionary<string, ParameterMetadata>(parameters.Length);
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            string parameterName;
            var commandOptionAttribute = parameter.GetCustomAttribute<CommandOptionAttribute>();
            if (commandOptionAttribute is not null)
            {
                parameterName = commandOptionAttribute.Alias is null ? parameter.Name : commandOptionAttribute.Alias;
            }
            else
            {
                parameterName = parameter.Name;
            }

            parameterMetadatas.Add(
                parameterName,
                new ParameterMetadata(
                    order: i,
                    name: parameterName,
                    canBeBull: CanBeBull(parameter.ParameterType),
                    isOptional: commandOptionAttribute.IsOptional,
                    dataType: parameter.ParameterType)
            );
        }

        _parameterMetadatas = new ReadOnlyDictionary<string, ParameterMetadata>(parameterMetadatas);
    }

    /// <summary>
    ///     Check whether <see cref="Type"/> is nullable
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns></returns>
    private bool CanBeBull(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition is not null && genericTypeDefinition == NullableType)
            {
                return true;
            }
        }

        if (type.IsAssignableTo(typeof(object)))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Executes command with given arguments
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="args"></param>
    /// <param name="parameterParser"></param>
    internal void Execute(
        IServiceProvider serviceProvider,
        CommandArgs args,
        ParameterParser parameterParser)
    {
        var executor = GetExecutorServiceInstance(serviceProvider);
        var parameters = new object[_parameterMetadatas.Count];
        FillOutCommandParameters(ref parameters, args, parameterParser);
        _methodToExecute.Invoke(executor, parameters);
    }

    /// <summary>
    ///     Executes command async with given arguments
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="args"></param>
    /// <param name="parameterParser"></param>
    internal async Task ExecuteAsync(
        IServiceProvider serviceProvider,
        CommandArgs args,
        ParameterParser parameterParser)
    {
        var executor = GetExecutorServiceInstance(serviceProvider);
        var parameters = new object[_parameterMetadatas.Count];
        FillOutCommandParameters(ref parameters, args, parameterParser);
        var task = (Task)_methodToExecute.Invoke(executor, parameters);
        if (task is null)
            throw new NullReferenceException("Couldn't get Task to execute");
        await task;
    }

    /// <summary>
    ///     Gets <see cref="CommandHandlerBase"/> instance class to execute
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/> instance to get service from</param>
    /// <returns></returns>
    private object GetExecutorServiceInstance(
        IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService(_handlerType);
    }

    private void FillOutCommandParameters(
        ref object[] parameters,
        CommandArgs args,
        ParameterParser parameterParser)
    {
        for (var i = 0; i < parameters.Length; i++)
        {
            var argMetaData = _parameterMetadatas.First(x => x.Value.Order == i);
            if (args.Args.TryGetValue(argMetaData.Key, out var argValue))
            {
                var value = parameterParser.Parse(argValue, argMetaData.Value.DataType);
                parameters[i] = value;
            }
            else if (argMetaData.Value.IsOptional)
            {
                if (argMetaData.Value.CanBeNull)
                {
                    parameters[i] = null;
                }
                else
                    parameters[i] = Activator.CreateInstance(argMetaData.Value.DataType);
            }
        }
    }
}