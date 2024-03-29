﻿namespace Console.Application.Parsing.TypeParsers;

internal class Int32InputParser : GenericInputParser<int>
{
    public Int32InputParser()
    {
    }

    protected override int ParseData(string input)
    {
        if (int.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Argument couldn't be converted to {ParsedType.Name}: {input}");
    }
}