using Docker.DotNet.Models;
using ContainerDeck.Shared.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ContainerDeck.Shared.Models;

public class Agent(string name, string address) {
    #region Public Properties --------------------------------------------------
    public string AgentIcon { get; set; } = GetRandomIcon();
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

    #region IconHandling -------------------------------------------------------

    public static string GetRandomIcon() {
        var icons = new List<string>
        {
            "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path d='M12 2L2 7v6c0 5.25 3.75 10 10 10s10-4.75 10-10V7l-10-5z'/></svg>",
            "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><circle cx='12' cy='12' r='10'/></svg>",
            "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><rect width='20' height='20' x='2' y='2' rx='5' ry='5'/></svg>",
            "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path d='M12 2L2 22h20L12 2z'/></svg>",
            "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path d='M12 2a10 10 0 100 20 10 10 0 000-20z'/></svg>"
        };

        var random = new Random();
        int index = random.Next(icons.Count);
        return icons[index];
    }

    #endregion

    #region Container Handling -------------------------------------------------
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

    public async Task StartContainer(string id) {
        _logger.LogDebug($"Starting container with ID: {id}");
        await GrpcWrapper.StartContainer(Address, id);
    }

    public async Task StopContainer(string id) {
        _logger.LogDebug($"Stopping container with ID: {id}");
        await GrpcWrapper.StopContainer(Address, id);
    }

    public async Task RestartContainer(string id) {
        _logger.LogDebug($"Restarting container with ID: {id}");
        await GrpcWrapper.RestartContainer(Address, id);
    }

    public async Task RemoveContainer(string id) {
        _logger.LogDebug($"Removing container with ID: {id}");
        await GrpcWrapper.RemoveContainer(Address, id);
    }

    public async Task RunNewContainer(Container container) {
        _logger.LogDebug($"Running new container on agent: {Name}");
        //await GrpcWrapper.RunNewContainer(Address, image, name, command, ports, volumes, environmentVariables); // TODO Implement
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
        var icon = root.GetProperty("AgentIcon").GetString();
        return new Agent(name, address) { AgentIcon = icon ?? string.Empty };
    }

    public override void Write(Utf8JsonWriter writer, Agent value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteString("Name", value.Name);
        writer.WriteString("Address", value.Address);
        writer.WriteString("AgentIcon", value.AgentIcon);
        writer.WriteEndObject();
    }
}