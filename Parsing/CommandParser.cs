using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Console.Application.Parsing;

internal class CommandParser
{
    private readonly string[] _availablePrefixes;

    internal CommandParser(string[] availablePrefixes)
    {
        _availablePrefixes = availablePrefixes;
    }

    internal CommandArgs ParseCommand(string text)
    {
        var args = SplitArgs(text);
        var firstArgIndex = FindFirstArg(ref args);
        string commandText;
        if (firstArgIndex != -1)
        {
            commandText = string.Join(' ', args[0..firstArgIndex]);
            var valuesLeft = args[firstArgIndex..];
            if (valuesLeft.Length % 2 != 0)
                throw new ArgumentException($"Incorrect amount of args: {valuesLeft.Length}");

            var argsDict = new Dictionary<string, string>();
            for (var i = 0; i < valuesLeft.Length; i += 2)
            {
                argsDict.Add(valuesLeft[i][1..], valuesLeft[i + 1]);
            }
            return new CommandArgs(commandText, argsDict);
        }

        commandText = string.Join(' ', args);
        return new CommandArgs(commandText, null);
    }

    private int FindFirstArg(ref string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            var value = args[i];
            if (IsArgName(value))
                return i;
        }

        return -1;
    }

    private bool IsArgName(string text)
    {
        for (var j = 0; j < _availablePrefixes.Length; j++)
        {
            if (text.StartsWith(_availablePrefixes[j]))
            {
                return true;
            }
        }

        return false;
    }

    private static string[] SplitArgs(string unsplitArgumentLine)
    {
        var ptrToSplitArgs = CommandLineToArgvW(unsplitArgumentLine, out var numberOfArgs);
        if (ptrToSplitArgs == IntPtr.Zero)
        {
            throw new ArgumentException("Unable to split argument.", new Win32Exception());
        }

        try
        {
            var splitArgs = new string[numberOfArgs];
            for (var i = 0; i < numberOfArgs; i++)
            {
                splitArgs[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));
            }

            return splitArgs;
        }
        finally
        {
            LocalFree(ptrToSplitArgs);
        }
    }

    [DllImport("shell32.dll", SetLastError = true)]
    private static extern IntPtr CommandLineToArgvW(
        [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
        out int pNumArgs);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr hMem);
}