
namespace PlainFiles.Core;

public class LogWriter : IDisposable
{
    private readonly StreamWriter _writer;

    public LogWriter(string path)
    {
        _writer = new StreamWriter(path, append: true)
        {
            AutoFlush = true   //si cierro el programa no se pierde info
        };
    }

    public void WriteLog(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("s"); // ISO 8601 format
        _writer.WriteLine($"[{timestamp}] - [{level}] - {message}");
    }

    public void Dispose()
    {
        _writer?.Dispose();
    }
}