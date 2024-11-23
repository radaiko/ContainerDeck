using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContainerDeck.Shared.Utils;

#region class Hub ----------------------------------------------------------------------------------
public static class Hub {

    #region Logging ------------------------------------------------------------
    private static List<ILogger> _loggers = new();
    private static ILogger GetLogger(string name) {
        var logger = _loggers.FirstOrDefault(l => l.GetType().Name == name);
        if (logger == null) {
            logger = new LoggerFactory().CreateLogger(name);
            _loggers.Add(logger);
        }
        return logger;
    }
    public static ILogger GetLogger(Type type) => new LoggerFactory().CreateLogger(type);

    public static ILogger<T> GetLogger<T>() => new LoggerFactory().CreateLogger<T>();

    public static void LogDebug(string message, [CallerMemberName] string caller = "") {
        var logger = GetLogger(caller);
        logger.LogDebug(message);
    }

    public static void LogWarning(string message, [CallerMemberName] string caller = "") {
        var logger = GetLogger(caller);
        logger.LogWarning(message);
    }
    public static void LogError(string message, [CallerMemberName] string caller = "") {
        var logger = GetLogger(caller);
        logger.LogError(message);
    }
    #endregion

    private static IConfigurationRoot GetConfig() {
        var config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .AddJsonFile("appsettings.Development.json", optional: true)
          .Build();
        return config;

    }

    public static string GetSettingsPath() {
        var config = GetConfig();
        var configFolder = config["ConfigFolder"];
        if (string.IsNullOrEmpty(configFolder)) {
            throw new InvalidOperationException("ConfigFolder is not set in the configuration.");
        }
        return Path.Combine(PathFix(configFolder), "settings.json");
    }

    public static string? ClientId { get; } = Guid.NewGuid().ToString();

    public static bool IsDevelopment() {
        var config = GetConfig();
        return config["Environment"] == "Development";
    }

    public static JsonSerializerOptions DefaultJsonOptions() => new() { WriteIndented = true };

    public static void FileWriteAllTextForce(string path, string content) {
        path = PathFix(path);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(path, content);
    }

    public static string PathFix(string path) {
        if (path.StartsWith('~')) {
            path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }
        return path;
    }
}
#endregion
