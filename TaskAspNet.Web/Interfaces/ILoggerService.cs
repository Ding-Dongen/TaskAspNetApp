

namespace TaskAspNet.Web.Interfaces;

public interface ILoggerService
{
    Task LogErrorAsync(string message);
    Task LogInfoAsync(string message);
}
