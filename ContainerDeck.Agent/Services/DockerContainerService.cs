using System.Text.Json;
using Agent.Helpers;
using ContainerDeck.Shared.Protos;
using ContainerDeck.Shared.Utils;
using Docker.DotNet.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ContainerDeck.Agent.Services;

public class DockerContainerService : DockerContainer.DockerContainerBase {

  private readonly ILogger logger = Hub.GetLogger<DockerContainerService>();

  public DockerContainerService() {
    logger.LogDebug("DockerContainerService created");
  }

  public override async Task<ProtoContainersResponse> GetContainers(Empty request, ServerCallContext context) {
    logger.LogDebug("GetContainers");
    var containers = await DockerWrapper.GetContainers();
    var response = new ProtoContainersResponse();
    foreach (var container in containers) {
      response.Containers.Add(await ParseContainer(container));
    }
    logger.LogDebug($"Return {response.Containers.Count} containers");
    return await Task.FromResult(response);
  }

  public override async Task<ProtoContainerResponse> GetContainer(ProtoContainerRequest request, ServerCallContext context) {
    logger.LogDebug("GetContainer");
    var container = await DockerWrapper.GetContainer(request.Id);
    return await ParseContainer(container);
  }

  private static async Task<ProtoContainerResponse> ParseContainer(ContainerListResponse? container) {
    if (container == null) return new ProtoContainerResponse();
    var containerInspect = await DockerWrapper.GetContainerInspect(container.ID);
    var containerStats = await DockerWrapper.GetContainerStats(container.ID);
    // config
    var config = new ProtoContainerConfig {
      Name = container.Names[0].TrimStart('/'),
      Image = container.Image,
      ImageId = container.ImageID,
      RestartPolicy = "",
    };
    config.Ports.AddRange(container.Ports.Select(p => $"{p.PublicPort}:{p.PrivatePort}"));
    config.Volumes.AddRange(container.Mounts.Select(m => $"{m.Source}:{m.Destination}"));
    config.Labels.AddRange(container.Labels.Select(l => $"{l.Key}={l.Value}"));

    // stats
    double cpuUsage = 0;
    if (containerStats.CPUStats == null || containerStats.CPUStats.OnlineCPUs == 0)
      cpuUsage = 0;
    else {
      var cpuDelta = containerStats.CPUStats.CPUUsage.TotalUsage - containerStats.PreCPUStats.CPUUsage.TotalUsage;
      var systemCpuDelta = containerStats.CPUStats.SystemUsage - containerStats.PreCPUStats.SystemUsage;
      cpuUsage = (cpuDelta / (double)systemCpuDelta * containerStats.CPUStats.OnlineCPUs * 100.0).Round(2);
    }

    var stats = new ProtoContainerStats();
    stats.Id = container.ID;
    stats.Status = container.State;
    stats.MemoryUsage = containerStats.MemoryStats == null ? 0 : containerStats.MemoryStats.Usage;
    stats.MemoryLimit = containerStats.MemoryStats == null ? 0 : containerStats.MemoryStats.Limit;
    stats.CpuUsage = cpuUsage;
    stats.LastStarted = Convert.ToDateTime(containerInspect.State.StartedAt).ToUniversalTime().ToProtoTimestamp();
    if (containerStats.Networks == null || containerStats.Networks.Count == 0) {
      stats.NetworkRx = 0;
      stats.NetworkTx = 0;
    }
    else {
      stats.NetworkRx = containerStats.Networks.FirstOrDefault().Value.RxBytes;
      stats.NetworkTx = containerStats.Networks.FirstOrDefault().Value.TxBytes;
    }

    // combined
    return new ProtoContainerResponse { Config = config, Stats = stats };
  }

  public override async Task<ProtoContainerInspectResponse> GetContainerInspect(ProtoContainerInspectRequest request, ServerCallContext context) {
    var inspect = await DockerWrapper.GetContainerInspect(request.Id);
    return new ProtoContainerInspectResponse() { Inspect = JsonSerializer.Serialize(inspect) };
  }

  public override async Task StreamLog(ProtoContainerLogRequest request, IServerStreamWriter<ProtoContainerLogResponse> responseStream, ServerCallContext context) {
    logger.LogDebug("Start StreamLog");
    var logs = DockerWrapper.Client.Containers.GetContainerLogsAsync(
      request.Id,
      new ContainerLogsParameters {
        ShowStdout = true,
        ShowStderr = true,
        Timestamps = true,
        Follow = true
      },
      context.CancellationToken,
      new Progress<string>(m => { responseStream.WriteAsync(new ProtoContainerLogResponse { Message = m }); })
    );
    await logs;
    logger.LogDebug("StreamLog ended");
  }

  public override async Task<Empty> StartContainer(ProtoContainerRequest request, ServerCallContext context) {
    await DockerWrapper.StartContainer(request.Id);
    return new Empty();
  }

  public override async Task<Empty> StopContainer(ProtoContainerRequest request, ServerCallContext context) {
    await DockerWrapper.StopContainer(request.Id);
    return new Empty();
  }

  public override async Task<Empty> RestartContainer(ProtoContainerRequest request, ServerCallContext context) {
    await DockerWrapper.RestartContainer(request.Id);
    return new Empty();
  }

  public override async Task<Empty> RemoveContainer(ProtoContainerRequest request, ServerCallContext context) {
    await DockerWrapper.RemoveContainer(request.Id);
    return new Empty();
  }
}