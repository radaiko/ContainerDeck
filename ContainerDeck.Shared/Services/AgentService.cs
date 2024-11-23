using System.Text.Json;
using ContainerDeck.Shared.Models;
using ContainerDeck.Shared.Utils;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;


namespace ContainerDeck.Shared.Services;

public class AgentsService {
    private List<Agent> _agents = [];
    private static readonly ILogger<AgentsService> _logger = Hub.GetLogger<AgentsService>();

    public event Action? AgentsChanged;
    private void NotifyPropertiesChanged() {
        _logger.LogDebug("NotifyPropertiesChanged");
        AgentsChanged?.Invoke();
    }

    public void Detach(Action action) {
        var delegates = AgentsChanged?.GetInvocationList().Where(a => a.Method.ReflectedType?.Name == action.Method.ReflectedType?.Name);
        if (delegates != null)
            foreach (var d in delegates.Cast<Action>()) {
                AgentsChanged -= d;
            }
    }

    public AgentsService() {
        Load();
    }

    #region Agent handling -------------------------------------------------------
    public void Add(Agent agent) {
        _agents.Add(agent);
        Save();
    }

    public void Delete(Agent agent) {
        _agents.Remove(agent);
        Save();
    }

    public Agent? GetAgentByContainerId(string containerId) {
        foreach (var agent in _agents.Where(agent => agent.DoesAgentContainsContainer(containerId))) {
            return agent;
        }
        return null;
    }

    #endregion

    #region Image handling -------------------------------------------------------
    public async Task<Image[]> GetImagesArray() {
        var images = await GetImagesList();
        return [.. images];
    }

    public async Task<List<Image>> GetImagesList() {
        List<Image> images = [];

        foreach (var agent in _agents) {
            images.AddRange(await GrpcWrapper.GetImages(agent.Address));
        }

        return images;
    }
    #endregion

    #region Container handling ---------------------------------------------------
    public async Task<Container[]> GetContainersArray() {
        var containers = await GetContainersList();
        return [.. containers];
    }
    public async Task<List<Container>> GetContainersList() {
        List<Container> containers = [];

        foreach (var agent in _agents) {
            containers.AddRange(await agent.GetContainersList());
        }

        return containers;
    }

    public async Task<Container> GetContainer(string id) {
        var agent = GetAgentByContainerId(id) ?? throw new InvalidOperationException($"No agent found for container ID {id}");
        return await agent.GetContainer(id);
    }

    public async Task<ContainerInspectResponse?> GetContainerInspect(string containerId) {
        var agent = GetAgentByContainerId(containerId);
        if (agent == null) return null;
        return await agent.GetContainerInspect(containerId);
    }

    public async Task<string> GetContainerInspectAsString(string containerId) =>
        JsonSerializer.Serialize(await GetContainerInspect(containerId), Hub.DefaultJsonOptions());

    public async IAsyncEnumerable<string> StreamContainerLogs(string containerId) {
        _logger.LogDebug("Start with streaming logs");
        var agent = GetAgentByContainerId(containerId);
        if (agent == null) yield break;
        await foreach (var log in agent.StreamContainerLogs(containerId)) {
            yield return log;
        }
    }
    #endregion

    #region Volume handling ------------------------------------------------------
    public async Task<Volume[]> GetVolumesArray() {
        var volumes = await GetVolumesList();
        return [.. volumes];
    }

    public async Task<List<Volume>> GetVolumesList() {
        List<Volume> volumes = [];

        foreach (var agent in _agents) {
            volumes.AddRange(await agent.GetVolumesList());
        }

        return volumes;
    }
    #endregion

    public Agent[] Get() => _agents.ToArray();

    #region Save and Load --------------------------------------------------------
    private void Save() {
        var options = Hub.DefaultJsonOptions();
        options.Converters.Add(new JsonConverterAgent());
        var json = JsonSerializer.Serialize(_agents, options);
        _logger.LogDebug($"Store settings: {json}");
        Hub.FileWriteAllTextForce(Hub.GetSettingsPath(), json);
        NotifyPropertiesChanged();
    }

    private void Load() {
        var path = Hub.GetSettingsPath();
        if (!File.Exists(path)) return;
        var json = File.ReadAllText(path);
        try {
            var agents = JsonSerializer.Deserialize<List<Agent>>(json, new JsonSerializerOptions { Converters = { new JsonConverterAgent() } });
            _agents = agents ?? [];
        }
        catch (Exception e) {
            _logger.LogError($"Error loading agents: {e.Message}");
            Save();
        }
        NotifyPropertiesChanged();
    }
    #endregion
}
