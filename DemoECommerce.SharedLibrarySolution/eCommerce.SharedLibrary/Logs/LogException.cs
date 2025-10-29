using Serilog;

namespace eCommerce.SharedLibrary.Logs;

public static class LogException
{
    public static void LogExceptions(Exception ex)
    {
        // Log exception details here
        LogToFile(ex.Message);
        LogToConsole(ex.Message);
        LogToDebugger(ex.Message);
    }

    public static void LogToDebugger(string message)
        => Log.Debug(message);

    public static void LogToConsole(string message)
        => Log.Warning(message);


    public static void LogToFile(string message)
        => Log.Information(message);

}
