using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContainerDeck.Shared.Utils;

#region class Hub ----------------------------------------------------------------------------------
public static class Hub
{

    #region Logging ------------------------------------------------------------
    private static List<ILogger> _loggers = new();
    private static ILogger GetLogger(string name)
    {
        var logger = _loggers.FirstOrDefault(l => l.GetType().Name == name);
        if (logger == null)
        {
            logger = new LoggerFactory().CreateLogger(name);
            _loggers.Add(logger);
        }
        return logger;
    }
    public static ILogger GetLogger(Type type) => new LoggerFactory().CreateLogger(type);

    public static ILogger<T> GetLogger<T>() => new LoggerFactory().CreateLogger<T>();

    public static void LogDebug(string message, [CallerMemberName] string caller = "")
    {
        var logger = GetLogger(caller);
        logger.LogDebug(message);
    }

    public static void LogWarning(string message, [CallerMemberName] string caller = "")
    {
        var logger = GetLogger(caller);
        logger.LogWarning(message);
    }
    public static void LogError(string message, [CallerMemberName] string caller = "")
    {
        var logger = GetLogger(caller);
        logger.LogError(message);
    }
    #endregion

    private static IConfigurationRoot GetConfig()
    {
        var config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .AddJsonFile("appsettings.Development.json", optional: true)
          .Build();
        return config;

    }

    public static string GetSettingsPath()
    {
        var config = GetConfig();
        return Path.Combine(config["ConfigFolder"] ?? string.Empty, "settings.json");
    }

    public static string? ClientId { get; } = Guid.NewGuid().ToString();

    public static bool IsDevelopment()
    {
        var config = GetConfig();
        return config["Environment"] == "Development";
    }
}
#endregion
