
using System.Text;
using TaskAspNet.Web.Interfaces;

namespace TaskAspNet.Web.Services;

public class LoggerService : ILoggerService
{
    private readonly string _logFilePath;

    public LoggerService(IWebHostEnvironment env)
    {
        var logDir = Path.Combine(env.ContentRootPath, "Logs");
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }

        _logFilePath = Path.Combine(logDir, $"log_{DateTime.Now:yyyyMMdd}.txt");
    }

    public async Task LogErrorAsync(string message)
    {
        await LogAsync("ERROR", message);
    }

    public async Task LogInfoAsync(string message)
    {
        await LogAsync("INFO", message);
    }

    private async Task LogAsync(string level, string message)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine("--------------------------------------------------");
        logEntry.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}]");
        logEntry.AppendLine(message);
        logEntry.AppendLine("--------------------------------------------------");

        await File.AppendAllTextAsync(_logFilePath, logEntry.ToString());
    }
}
