using System.Text.Json;
using Docker.DotNet.Models;
using ContainerDeck.Shared.Models;
using ContainerDeck.Shared.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ContainerDeck.Shared.Utils;

#region class GrpcWrapper --------------------------------------------------------------------------
public static class GrpcWrapper {
    private static CancellationTokenSource? _logCancellationToken = new() { };
    private static ILogger Logger {
        get {
            _logger ??= Hub.GetLogger(typeof(GrpcWrapper));
            return _logger;
        }
    }
    private static ILogger? _logger = null;


    #region Cancelation ----------------------------------------------------------
    public static void CancelLogs() {
        _logCancellationToken?.Cancel();
        _logCancellationToken = new();
    }
    #endregion

    #region Base Endpoints -------------------------------------------------------
    public static async Task<bool> IsAgentHealthy(string baseUrl) {
        try {
            var client = GrpcFactory.GetClient<Health.HealthClient>(baseUrl);
            var health = await client.GetHealthAsync(new());
            Logger.LogInformation($"Health for {baseUrl}: {health.Status}");
            return health.Status == HealthStatus.Healthy;
        }
        catch {
            return false;
        }
    }
    public static async Task<string> GetDockerVersion(string baseUrl) {
        try {
            var client = GrpcFactory.GetClient<DockerSystem.DockerSystemClient>(baseUrl);
            Logger.LogInformation($"Getting Docker version for {baseUrl}");
            var version = await client.GetVersionAsync(new());
            Logger.LogInformation($"Got Docker version for {baseUrl}: {version.VersionString}");
            return version.VersionString;
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Failed to get Docker version");
            return "Error";
        }
    }
    public static async Task<string> GetAgentVersion(string baseUrl) {
        return await Task.FromResult("Not implemented");
    }

    public static async IAsyncEnumerable<LogEntry> GetAgentLogs(string baseUrl) {
        var client = GrpcFactory.GetClient<Log.LogClient>(baseUrl);

        // existing logs
        var existingLogs = client.GetLogs(new LogRequest() { ClientId = Hub.ClientId });
        foreach (var entry in existingLogs.Entries) {
            yield return entry;
        }

        // new logs
        using AsyncServerStreamingCall<LogEntry>? stream = client.StreamLogs(new LogRequest() { ClientId = Hub.ClientId });
        while (_logCancellationToken is { IsCancellationRequested: false }) {
            while (await stream.ResponseStream.MoveNext()) {
                var entry = stream.ResponseStream.Current;
                yield return entry;
            }
        }
        Logger.LogDebug("Getting logs ended");
    }
    #endregion

    #region Image Endpoints ------------------------------------------------------
    public static async Task<Image[]> GetImages(string baseUrl) {
        var client = GrpcFactory.GetClient<DockerImage.DockerImageClient>(baseUrl);
        var images = await client.GetImagesAsync(new());
        Logger.LogDebug($"Got images for {baseUrl}: {images.Images.Count}");
        return images.Images.Select(image => new Image(image)).ToArray();
    }
    #endregion

    #region Container Endpoints --------------------------------------------------
    public static async Task<Container[]> GetContainers(string baseUrl) {
        var client = GrpcFactory.GetClient<DockerContainer.DockerContainerClient>(baseUrl);
        var containers = await client.GetContainersAsync(new());
        Logger.LogDebug($"Got containers for {baseUrl}: {containers.Containers.Count}");
        return containers.Containers.Select(container => new Container(container)).ToArray();
    }

    public static async Task<Container> GetContainer(string baseUrl, string containerId) {
        var client = GrpcFactory.GetClient<DockerContainer.DockerContainerClient>(baseUrl);
        var container = await client.GetContainerAsync(new ProtoContainerRequest() { Id = containerId });
        return new Container(container);
    }

    public static async Task<ContainerInspectResponse?> GetContainerInspect(string baseUrl, string containerId) {
        var client = GrpcFactory.GetClient<DockerContainer.DockerContainerClient>(baseUrl);
        var containerInspect = await client.GetContainerInspectAsync(new ProtoContainerInspectRequest() { Id = containerId });
        return JsonSerializer.Deserialize<ContainerInspectResponse>(containerInspect.Inspect);
    }

    public static async IAsyncEnumerable<string> StreamLogs(string baseUrl, string containerId) {
        Logger.LogDebug("Start with streaming logs");
        var client = GrpcFactory.GetClient<DockerContainer.DockerContainerClient>(baseUrl);
        Logger.LogDebug("Got remote client");
        using AsyncServerStreamingCall<ProtoContainerLogResponse>? stream = client.StreamLog(new ProtoContainerLogRequest() { Id = containerId });
        Logger.LogDebug("Got remote stream");
        while (await stream.ResponseStream.MoveNext()) {
            var entry = stream.ResponseStream.Current;
            yield return entry.Message;
        }
    }

    public static async Task StartContainer(string baseUrl, string containerId) {
        var client = GrpcFactory.GetClient<DockerContainer.DockerContainerClient>(baseUrl);
        await client.StartContainerAsync(new ProtoContainerRequest() { Id = containerId });
    }

    public static async Task StopContainer(string baseUrl, string containerId) {
        var client = GrpcFactory.GetClient<DockerContainer.DockerContainerClient>(baseUrl);
        await client.StopContainerAsync(new ProtoContainerRequest() { Id = containerId });
    }

    public static async Task RestartContainer(string baseUrl, string containerId) {
        var client = GrpcFactory.GetClient<DockerContainer.DockerContainerClient>(baseUrl);
        await client.RestartContainerAsync(new ProtoContainerRequest() { Id = containerId });
    }

    public static async Task RemoveContainer(string baseUrl, string containerId) {
        var client = GrpcFactory.GetClient<DockerContainer.DockerContainerClient>(baseUrl);
        await client.RemoveContainerAsync(new ProtoContainerRequest() { Id = containerId });
    }
    #endregion

    #region Volume Endpoints -----------------------------------------------------
    public static async Task<Volume[]> GetVolumes(string baseUrl) {
        var client = GrpcFactory.GetClient<DockerVolume.DockerVolumeClient>(baseUrl);
        var volumes = await client.GetVolumesAsync(new());
        Logger.LogDebug($"Got volumes for {baseUrl}: {volumes.Volumes.Count}");
        return volumes.Volumes.Select(volume => new Volume(volume)).ToArray();
    }
    #endregion
}
#endregion

#region class GrpcFactory --------------------------------------------------------------------------
public static class GrpcFactory {
    private static List<GChannel> _channels = [];
    private static ILogger Logger {
        get {
            _logger ??= Hub.GetLogger(typeof(GrpcFactory));
            return _logger;
        }
    }
    private static ILogger? _logger = null;

    public static T GetClient<T>(string url) where T : ClientBase<T> {
        var gChannel = _channels.FirstOrDefault(c => c.Url == url);

        // unknown channel
        if (gChannel == null) {
            Logger.LogDebug($"Creating channel for {url}");
            var channel = GrpcChannel.ForAddress(url);
            gChannel = new GChannel(url, channel, [CreateClient<T>(channel)]);
            _channels.Add(gChannel);
            return gChannel.Clients.OfType<T>().FirstOrDefault()!;
        }

        // channel known
        Logger.LogDebug("Channel found");
        var client = gChannel.Clients.OfType<T>().FirstOrDefault();
        if (client != null) {
            Logger.LogDebug("Client found");
            return client;
        }

        // no client found
        client = CreateClient<T>(gChannel.Channel);
        gChannel.Clients.Add(client);
        return client;
    }

    private static T CreateClient<T>(GrpcChannel channel) where T : ClientBase<T> {
        Logger.LogDebug($"Creating client for {typeof(T).Name}");
        return (T)Activator.CreateInstance(typeof(T), channel)!;
    }

    internal class GChannel(string url, GrpcChannel grpcChannel, List<ClientBase> clients) {
        internal string Url { get; set; } = url;
        internal GrpcChannel Channel { get; set; } = grpcChannel;
        internal List<ClientBase> Clients { get; set; } = clients;
    }
}
#endregion
