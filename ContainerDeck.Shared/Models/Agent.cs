using Docker.DotNet.Models;
using ContainerDeck.Shared.Models;
using ContainerDeck.Shared.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ContainerDeck.Shared.Models;

public class Agent(string name, string address) {
    #region Public Properties --------------------------------------------------
    public Icon AgentIcon { get; set; } = GetRandomIcon();
    public string Name { get; set; } = name;
    public string Address { get; set; } = address;
    #endregion

    #region Public Properties Methods ------------------------------------------
    public async Task<bool> IsHealthy() => await GrpcWrapper.IsAgentHealthy(Address);
    public async Task<string> DockerVersion() => await GrpcWrapper.GetDockerVersion(Address);
    public async Task<string> AgentVersion() => await GrpcWrapper.GetAgentVersion(Address);
    #endregion

    #region Private fields -----------------------------------------------------
    readonly List<string> _containerIds = [];
    private readonly ILogger _logger = Hub.GetLogger(typeof(Agent));
    #endregion

    #region IconHandling ------------------------------------------------------

    public static Icon GetRandomIcon() {
        var classesOfNamespace = typeof(Icons.Regular.Size32).GetNestedTypes();
        var randomIndex = new Random().Next(0, classesOfNamespace.Length);
        var selectedClass = classesOfNamespace[randomIndex];
        return (Icon)Activator.CreateInstance(selectedClass)!;
    }

    #endregion

    #region ContainerHandling --------------------------------------------------
    public async Task<List<Container>> GetContainersList() {
        List<Container> containers = [];
        _logger.LogDebug($"Fetching containers for agent: {Name}");
        containers.AddRange(await GrpcWrapper.GetContainers(Address));
        _containerIds.Clear();
        foreach (var container in containers) {
            _logger.LogDebug($"Found container with ID: {container.Id}");
            if (container.Id != null) _containerIds.Add(container.Id);
        }
        return containers;
    }

    public async Task<Container> GetContainer(string id) {
        _logger.LogDebug($"Fetching container with ID: {id}");
        var container = await GrpcWrapper.GetContainer(Address, id);
        return container;
    }

    public bool DoesAgentContainsContainer(string containerId) {
        _logger.LogDebug($"Checking if agent contains container with ID: {containerId}");
        return _containerIds.Contains(containerId);
    }

    public async IAsyncEnumerable<string> StreamContainerLogs(string containerId) {
        _logger.LogDebug("Start with streaming logs");
        await foreach (var log in GrpcWrapper.StreamLogs(Address, containerId)) {
            yield return log;
        }
    }

    public async Task<ContainerInspectResponse?> GetContainerInspect(string id) {
        _logger.LogDebug($"Fetching container inspect for ID: {id}");
        return await GrpcWrapper.GetContainerInspect(Address, id);
    }
    #endregion

    #region VolumeHandling -----------------------------------------------------
    public async Task<IEnumerable<Volume>> GetVolumesList() {
        _logger.LogDebug($"Fetching volumes for agent: {Name}");
        return await GrpcWrapper.GetVolumes(Address);
    }

    #endregion
}

public class JsonConverterAgent : JsonConverter<Agent> {
    public override Agent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var json = JsonDocument.ParseValue(ref reader);
        var root = json.RootElement;
        var name = root.GetProperty("Name").GetString() ?? throw new JsonException("Name property is missing or null.");
        var address = root.GetProperty("Address").GetString() ?? throw new JsonException("Address property is missing or null.");
        var iconName = root.GetProperty("AgentIcon").GetString();
        var iconType = typeof(Icons.Regular.Size32).GetNestedTypes().FirstOrDefault(t => t.Name == iconName) ?? throw new JsonException($"Icon type {iconName} not found.");
        var icon = (Icon)Activator.CreateInstance(iconType)!;
        return new Agent(name, address) { AgentIcon = icon };
    }

    public override void Write(Utf8JsonWriter writer, Agent value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteString("Name", value.Name);
        writer.WriteString("Address", value.Address);
        writer.WriteString("AgentIcon", value.AgentIcon.Name);
        writer.WriteEndObject();
    }
}