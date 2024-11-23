namespace ContainerDeck.Agent.Services;

using ContainerDeck.Shared.Protos;
using ContainerDeck.Shared.Utils;
using Grpc.Core;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class LogService : Log.LogBase {
  private static ConcurrentDictionary<string, IServerStreamWriter<LogEntry>> _clients = new();
  private static bool _isAttached = false;

  private readonly ILogger logger = Hub.GetLogger<LogService>();

  public LogService() {
    logger.LogDebug("LogService created");
  }

  public override Task<LogResponse> GetLogs(LogRequest request, ServerCallContext context) {
    var clientId = request.ClientId;
    logger.LogDebug($"GetLogs for {clientId}");
    var response = new LogResponse();
    // TODO: add logic to get logs
    // foreach (var entry in RHistory.GetHistory()) {
    //   response.Entries.Add(entry.ToLogEntry());
    // }
    return Task.FromResult(response);
  }

  public override async Task StreamLogs(LogRequest request, IServerStreamWriter<LogEntry> responseStream, ServerCallContext context) {
    var clientId = request.ClientId;

    _clients[clientId] = responseStream;
    if (!_isAttached) {
      // TODO: add logic to attach to log events
      // RHistory.OnLogEntryAdded += async (logEntry) => wait{ SendLogEntryToClients(logEntry); };
    }

    while (!context.CancellationToken.IsCancellationRequested) {
      await Task.Delay(1);
    }

    _clients.TryRemove(clientId, out _);
  }

  public static void SendLogEntryToClients(LogEntry logEntry) {
    foreach (var client in _clients.Values) {
      client.WriteAsync(logEntry);
    }
  }
}