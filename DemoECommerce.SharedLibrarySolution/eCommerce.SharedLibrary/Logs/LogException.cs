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

    private static void LogToDebugger(string message)
        => Log.Debug(message);

    private static void LogToConsole(string message)
        => Log.Warning(message);


    private static void LogToFile(string message)
        => Log.Information(message);

}
