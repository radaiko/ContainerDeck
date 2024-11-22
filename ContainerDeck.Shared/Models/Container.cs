using System.Text.Json.Serialization;
using Docker.DotNet.Models;
using ContainerDeck.Shared.Utils;
using ContainerDeck.Shared.Protos;

namespace ContainerDeck.Shared.Models;

#region class Container ----------------------------------------------------------------------------
public class Container
{
  #region Properties -----------------------------------------------------------
  // Stats
  public string? Id { get; set; }

  public ContainerStatus Status { get; set; } = ContainerStatus.Unknown;
  public double MemoryUsage { get; set; } = 0;
  public double MemoryLimit { get; set; } = 0;
  public DateTime? LastStarted { get; set; }
  public double CpuUsage { get; set; } = 0;
  public ulong? NetworkUsage { get; set; } = 0;
  public string[] Logs { get; set; } = [];
  public List<Statistic> Statistics { get; set; } = [];
  public bool IsUpdateAvailable { get; set; }

  // Config
  public string? Name { get; set; }
  public string? Image { get; set; }

  public string? ImageId { get; set; }
  public List<PortMapping> Ports { get; set; } = [];
  public List<VolumeMapping> Volumes { get; set; } = [];
  public RestartPolicyKind Restart { get; set; }
  public List<EnvironmentMapping> Labels { get; set; } = [];
  #endregion

  #region Stats Methods --------------------------------------------------------
  public string MemoryUsageFormatted() => ((long)MemoryUsage).ToFileSize();
  public string MemoryLimitFormatted() => ((long)MemoryLimit).ToFileSize();
  public string CpuUsageFormatted() => $"{CpuUsage}%";
  public string PortsFormatted() => string.Join(", ", Ports);
  public TimeSpan? Age() => DateTime.Now - LastStarted;
  public string AgeFormatted() => Age()?.ToReadableString() ?? "";
  public bool IsRunning() => Status == ContainerStatus.Running || Status == ContainerStatus.Restarting;
  public bool IsComposed() => !string.IsNullOrEmpty(ComposeName());
  public string? ComposeName() => Labels.FirstOrDefault(l => l.Name == "com.docker.compose.project")?.Value;
  public string? ComposeFile() => Labels.FirstOrDefault(l => l.Name == "com.docker.compose.project.config_files")?.Value;
  public string? ComposeService() => Labels.FirstOrDefault(l => l.Name == "com.docker.compose.service")?.Value;
  #endregion

  #region Constructor ----------------------------------------------------------
  [JsonConstructor]
  public Container() { }

  public Container(ContainerListResponse baseInfo, ContainerStatsResponse stats, ContainerInspectResponse inspect, string[] logs)
  {
    // Config
    Id = baseInfo.ID;
    Name = baseInfo.Names[0].TrimStart('/');
    Image = baseInfo.Image;
    ImageId = baseInfo.ImageID;
    Status = baseInfo.State.Equals("exited", StringComparison.OrdinalIgnoreCase)
      ? ContainerStatus.Unknown
      : Enum.Parse<ContainerStatus>(baseInfo.State, true);
    foreach (var port in baseInfo.Ports)
    {
      if (port.PrivatePort == 0 || port.PublicPort == 0) continue;
      Ports.Add(new PortMapping { HostPort = port.PublicPort, ContainerPort = port.PrivatePort });
    }
    foreach (var mounts in baseInfo.Mounts)
    {
      if (mounts == null) continue;
      Volumes.Add(new VolumeMapping { HostPath = mounts.Source, ContainerPath = mounts.Destination }); ;
    }
    foreach (var kv in baseInfo.Labels)
    {
      Labels.Add(new EnvironmentMapping { Name = kv.Key, Value = kv.Value });
    }

    // Stats
    MemoryUsage = stats.MemoryStats.Usage;
    MemoryLimit = stats.MemoryStats.Limit;
    if (stats.CPUStats.OnlineCPUs == 0)
      CpuUsage = 0;
    else
    {
      var cpuDelta = stats.CPUStats.CPUUsage.TotalUsage - stats.PreCPUStats.CPUUsage.TotalUsage;
      var systemCpuDelta = stats.CPUStats.SystemUsage - stats.PreCPUStats.SystemUsage;
      CpuUsage = (cpuDelta / (double)systemCpuDelta * stats.CPUStats.OnlineCPUs * 100.0).Round(2);
    }
    NetworkUsage = 0;
    if (stats.Networks == null || !stats.Networks.Any()) return;
    foreach (var network in stats.Networks)
    {
      var rxBytes = network.Value.RxBytes;
      var txBytes = network.Value.TxBytes;
      NetworkUsage += rxBytes + txBytes;
    }
    var lastStarted = Convert.ToDateTime(inspect.State.StartedAt);
    if (lastStarted != DateTime.MinValue)
      LastStarted = lastStarted;
    Logs = logs;
  }

  public Container(ProtoContainerResponse protoContainer)
  {
    Id = protoContainer.Stats.Id;
    Name = protoContainer.Config.Name;
    Image = protoContainer.Config.Image;
    ImageId = protoContainer.Config.ImageId;
    ContainerStatus containerStatus = ContainerStatus.Unknown;
    if (protoContainer.Stats.Status != null) Enum.TryParse(protoContainer.Stats.Status, true, out containerStatus);
    Status = containerStatus;
    MemoryUsage = protoContainer.Stats.MemoryUsage;
    MemoryLimit = protoContainer.Stats.MemoryLimit;
    CpuUsage = protoContainer.Stats.CpuUsage;
    LastStarted = protoContainer.Stats.LastStarted.ToDateTime();
    NetworkUsage = protoContainer.Stats.NetworkRx + protoContainer.Stats.NetworkTx;
    Restart = protoContainer.Config.RestartPolicy switch
    {
      "always" => RestartPolicyKind.Always,
      "unless-stopped" => RestartPolicyKind.UnlessStopped,
      "on-failure" => RestartPolicyKind.OnFailure,
      _ => RestartPolicyKind.No
    };
    foreach (var port in protoContainer.Config.Ports)
    {
      var parts = port.Split(':');
      Ports.Add(new PortMapping { HostPort = Convert.ToInt32(parts[0]), ContainerPort = Convert.ToInt32(parts[1]) });
    }
    foreach (var volume in protoContainer.Config.Volumes)
    {
      var parts = volume.Split(':');
      Volumes.Add(new VolumeMapping { HostPath = parts[0], ContainerPath = parts[1] });
    }
    foreach (var label in protoContainer.Config.Labels)
    {
      var parts = label.Split('=');
      Labels.Add(new EnvironmentMapping { Name = parts[0], Value = parts[1] });
    }
  }
  #endregion

  #region Public Methods -------------------------------------------------------
  public void StoreStatistic()
  {
    Statistics.Add(new Statistic { CpuUsage = CpuUsage, MemoryUsage = MemoryUsage });
  }

  public void Merge(Container container)
  {
    Status = container.Status;
    MemoryUsage = container.MemoryUsage;
    MemoryLimit = container.MemoryLimit;
    LastStarted = container.LastStarted;
    CpuUsage = container.CpuUsage;
    NetworkUsage = container.NetworkUsage;
    Logs = container.Logs;
    Statistics = container.Statistics;
    Ports = container.Ports;
    Volumes = container.Volumes;
    Labels = container.Labels;
    Restart = container.Restart;
  }
  #endregion

  #region Nested Classes ---------------------------------------------------------------------------

  #region class PortMapping ------------------------------------------------------------------------
  public class PortMapping()
  {
    public int? HostPort { get; set; }
    public int? ContainerPort { get; set; }

    public override string ToString() => $"{HostPort}:{ContainerPort}";
  }
  #endregion

  #region class VolumeMapping ----------------------------------------------------------------------
  public class VolumeMapping()
  {
    public string? HostPath { get; set; }
    public string? ContainerPath { get; set; }
    public override string ToString() => $"{HostPath}:{ContainerPath}";
  }
  #endregion

  #region class EnvironmentMapping -----------------------------------------------------------------
  public class EnvironmentMapping()
  {
    public string? Name { get; set; }
    public string? Value { get; set; }
    public override string ToString() => $"{Name}={Value}";
  }
  #endregion

  #region class Statistic --------------------------------------------------------------------------
  public class Statistic()
  {
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
  }
  #endregion

  #endregion
}
#endregion

#region class Containers ---------------------------------------------------------------------------
public class Containers
{
  #region Private Properties ---------------------------------------------------
  [JsonInclude]
  private List<Container> _containers { get; set; } = [];
  #endregion

  #region Public Methods -------------------------------------------------------
  public void Clear()
  {
    _containers.Clear();
  }

  public void MergeContainers(List<Container> containers)
  {
    // Remove containers not in the new list
    _containers.RemoveAll(c => containers.All(newContainer => newContainer.Id != c.Id));

    // Add or update containers
    foreach (var container in containers)
    {
      var existingContainer = _containers.FirstOrDefault(c => c.Id == container.Id);
      if (existingContainer != null)
      {
        existingContainer.Merge(container);
      }
      else
      {
        _containers.Add(container);
      }
    }
  }
  public void StoreStatistics()
  {
    foreach (Container container in _containers)
    {
      container.StoreStatistic();
    }
  }
  public List<Container> GetAll() => _containers.OrderByDescending(c => c.Id).ToList();
  public int Count() => _containers.Count;
  public int RunningCount() => _containers.Count(c => c.Status == ContainerStatus.Running);
  public int StoppedCount() => _containers.Count(c => c.Status == ContainerStatus.Exited);
  public int PausedCount() => _containers.Count(c => c.Status == ContainerStatus.Paused);
  public int RestartingCount() => _containers.Count(c => c.Status == ContainerStatus.Restarting);
  public int DeadCount() => _containers.Count(c => c.Status == ContainerStatus.Dead);
  public double CpuUsage() => _containers.Sum(c => c.CpuUsage);
  public long MemoryUsage() => _containers.Sum(c => (long)c.MemoryUsage);
  public string MemoryUsageFormatted() => MemoryUsage().ToFileSize();
  public long MemoryLimit() => _containers.Sum(c => (long)c.MemoryLimit);
  public string MemoryLimitFormatted() => MemoryLimit().ToFileSize();

  #endregion
}
#endregion

#region enum ContainerStatus -----------------------------------------------------------------------
public enum ContainerStatus
{
  Unknown,
  Running,
  Exited,
  Paused,
  Restarting,
  Dead
}
#endregion
