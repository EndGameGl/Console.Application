using System.Collections.ObjectModel;
using Console.Application.Parsing.TypeParsers;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Application.Parsing;

internal class ParameterParser
{
    private readonly ReadOnlyDictionary<Type, InputParser> _parsers;

    internal ParameterParser(
        IServiceProvider serviceProvider,
        ICollection<Type> typeParsers)
    {
        var parsers = new Dictionary<Type, InputParser>(typeParsers.Count);
        foreach (var typeParser in typeParsers)
        {
            var parser = (InputParser)serviceProvider.GetRequiredService(typeParser);
            parsers.Add(parser.ParsedType, parser);
        }

        _parsers = new ReadOnlyDictionary<Type, InputParser>(parsers);
    }

    internal object Parse(string data, Type dataType)
    {
        if (_parsers.TryGetValue(dataType, out var parser))
        {
            return parser.Parse(data);
        }

        throw new ArgumentException("No parser for this kind of data is registered");
    }
}