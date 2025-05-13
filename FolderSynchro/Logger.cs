
using System;
using System.IO;

public class Logger
{
    private readonly string _logFilePath;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
    }

    public void Log(string message)
    {
        string entry = $"{DateTime.Now:G} - {message}";
        Console.WriteLine(entry);
        File.AppendAllText(_logFilePath, entry + Environment.NewLine);
    }
}
