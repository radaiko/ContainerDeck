using Docker.DotNet.Models;
using ContainerDeck.Shared.Models;
using ContainerDeck.Shared.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;

namespace ContainerDeck.Shared;

public class Agent {
    #region Constructors -------------------------------------------------------
    public Agent(string name, string address) {
        Name = name;
        Address = address;
        AgentIcon = GetRandomIcon();
    }
    #endregion

    #region Public Properties --------------------------------------------------
    public Icon AgentIcon { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<string> Logs { get; set; } = [];
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
        return (Icon)(typeof(Icons.Regular.Size48).GetFields()[new Random().Next(0, typeof(Icons.Regular.Size48).GetFields().Length)].GetValue(null) ?? new Icon());
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