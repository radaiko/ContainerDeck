using System.Runtime.CompilerServices;
using ContainerDeck.Shared.Utils;
using Docker.DotNet;
using Docker.DotNet.Models;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace Agent.Helpers;

public static class DockerWrapper {

  private static readonly ILogger logger = Hub.GetLogger(typeof(DockerWrapper));

  public static DockerClient Client {
    get {
      if (_client == null) {
        logger.LogInformation("Creating Docker client");
        _client = new DockerClientConfiguration().CreateClient();
        if (_client == null) {
          logger.LogError("Failed to create Docker client");
          throw new Exception("Failed to create Docker client");
        }
        logger.LogInformation("Docker client created");
      }
      return _client;
    }
  }
  private static DockerClient? _client;

  #region Container Methods ----------------------------------------------------
  internal static async Task<IList<ContainerListResponse?>> GetContainers() {
    // logger.LogDebug("Getting containers");
    var result = await Client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
    // logger.LogDebug($"Got {result.Count} containers");
    return result;
  }
  internal static async Task<ContainerListResponse?> GetContainer(string containerId) {
    return (await GetContainers()).FirstOrDefault(container => container?.ID == containerId);
  }
  internal static async Task<List<ContainerStatsResponse>> GetContainersStats(IList<ContainerListResponse> containers) {
    // logger.LogDebug("Getting containers stats");
    var containerStatsList = new List<ContainerStatsResponse>();
    await Task.WhenAll(containers.Select(async container => {
      var stats = await GetContainerStats(container.ID);
      lock (containerStatsList) {
        containerStatsList.Add(stats);
      }
    }));
    return containerStatsList;
  }
  internal static async Task<ContainerStatsResponse> GetContainerStats(string containerId) {
    ContainerStatsResponse? containerStats = null;
    await Client.Containers.GetContainerStatsAsync(
      containerId,
      new ContainerStatsParameters {
        Stream = false,
        OneShot = true
      },
      new Progress<ContainerStatsResponse>(m => { containerStats = m; })
    );
    await Task.Delay(TimeSpan.FromMilliseconds(1));
    return containerStats ?? new ContainerStatsResponse();
  }
  internal static async Task<List<ContainerInspectResponse>> GetContainersInspect(IList<ContainerListResponse> containers) {
    // logger.LogDebug("Getting containers inspect");
    var containerInspectList = new List<ContainerInspectResponse>();
    foreach (var container in containers) {
      containerInspectList.Add(await GetContainerInspect(container.ID));
    }
    return containerInspectList;
  }
  internal static async Task<ContainerInspectResponse> GetContainerInspect(string containerId) {
    return await Client.Containers.InspectContainerAsync(containerId);
  }
  internal static async Task StartContainer(string containerId) {
    await Client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
  }
  internal static async Task StopContainer(string containerId) {
    await Client.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
  }
  internal static async Task RestartContainer(string containerId) {
    await Client.Containers.RestartContainerAsync(containerId, new ContainerRestartParameters());
  }
  internal static async Task RemoveContainer(string containerId) {
    await Client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters());
  }
  internal static async Task<MultiplexedStream> GetContainerLogs(string containerId, CancellationToken cancellationToken) {
    var logs = await Client.Containers.GetContainerLogsAsync(containerId, true, new ContainerLogsParameters() {
      ShowStdout = true,
      ShowStderr = true,
      Timestamps = true,
      Follow = true
    }, cancellationToken);
    return logs;
  }

  internal static async IAsyncEnumerable<string?> StreamContainerLogs(string containerId, [EnumeratorCancellation] CancellationToken cancellationToken) {
    logger.LogDebug("Start with streaming logs");
    Queue<string> logList = new Queue<string>(1000);
    var logs = Client.Containers.GetContainerLogsAsync(
      containerId,
      new ContainerLogsParameters() {
        ShowStdout = true,
        ShowStderr = true,
        Timestamps = true,
        Follow = true
      },
      cancellationToken,
      new Progress<string>(m => { logList.Enqueue(m); })
      );

    await Task.Delay(10, cancellationToken);
    while (logs.Status == TaskStatus.Running) {
      yield return logList.Dequeue();
      await Task.Delay(1, cancellationToken);
    }

    // logger.LogDebug("Got logs multiplexed stream");
    // await Task.Delay(10, cancellationToken);
    // logger.LogDebug("Delay ended");
    // using var memoryStream = new MemoryStream();
    // await logs.CopyOutputToAsync(default, memoryStream, memoryStream, default);
    // logger.LogDebug("Copied output to memory stream");
    // memoryStream.Position = 0;
    // using var reader = new StreamReader(memoryStream);
    // while (!reader.EndOfStream) {
    //   logger.LogDebug("Got log entry, yield return it");
    //   yield return await reader.ReadLineAsync(cancellationToken);
    // }
    // logger.LogDebug("Getting logs ended");
  }

  internal static async Task<string[]> GetContainerLogsTakeLast(string containerId, int lines) {
    var logs = await Client.Containers.GetContainerLogsAsync(containerId, true, new ContainerLogsParameters() {
      ShowStdout = true,
      ShowStderr = true,
      Timestamps = true,
      Tail = lines.ToString()
    });
    await Task.Delay(500);
    using var memoryStream = new MemoryStream();
    await logs.CopyOutputToAsync(default, memoryStream, memoryStream, default);
    memoryStream.Position = 0;
    using var reader = new StreamReader(memoryStream);
    var content = await reader.ReadToEndAsync();
    logger.LogDebug($"Got {content.Length} bytes of logs");
    return content.Split('\n');
  }
  internal static async Task<string> GetContainerLogsAsString(string containerId) {
    var logs = await Client.Containers.GetContainerLogsAsync(containerId, true, new ContainerLogsParameters() {
      ShowStdout = true,
      ShowStderr = true
    });
    await Task.Delay(500);
    using var memoryStream = new MemoryStream();
    await logs.CopyOutputToAsync(default, memoryStream, memoryStream, default);
    memoryStream.Position = 0;
    using var reader = new StreamReader(memoryStream);
    var content = await reader.ReadToEndAsync();
    logger.LogDebug($"Got {content.Length} bytes of logs");
    return content;
  }
  internal static async Task<bool> IsContainerUpToDate(string containerId) {
    // if a new image is available
    var container = await Client.Containers.InspectContainerAsync(containerId);
    var image = await Client.Images.InspectImageAsync(container.Image);
    return container.Image == image.ID;
  }
  #endregion

  #region Image Methods --------------------------------------------------------
  internal static async Task<IList<ImagesListResponse?>> GetImages() {
    return await Client.Images.ListImagesAsync(new ImagesListParameters());
  }
  internal static async Task<ImagesListResponse?> GetImage(string imageId) {
    return (await GetImages()).FirstOrDefault(image => image?.ID == imageId);
  }
  internal static async Task<List<ImageInspectResponse>> GetImagesInspects(IList<ImagesListResponse> images) {
    var imagesStats = new List<ImageInspectResponse>();
    foreach (var image in images) {
      imagesStats.Add(await GetImageInspect(image.ID));
    }
    return imagesStats;
  }
  private static async Task<ImageInspectResponse> GetImageInspect(string imageId) {
    return await Client.Images.InspectImageAsync(imageId);
  }
  internal static async Task RemoveImage(string imageId) {
    await Client.Images.DeleteImageAsync(imageId, new ImageDeleteParameters());
  }
  internal static async Task PullImage(string imageName) {
    logger.LogInformation($"Pull image: {imageName}");
    await Client.Images.CreateImageAsync(
      new ImagesCreateParameters { FromImage = imageName },
      null,
      new Progress<JSONMessage>(message => logger.LogInformation($"Pull: {imageName} - {message.Status}")),
      new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token
    );
  }

  #endregion

  #region System Methods -------------------------------------------------------
  internal static async Task<VersionResponse> GetVersion() => await Client.System.GetVersionAsync();
  #endregion

  #region Volumes Methods ------------------------------------------------------
  internal static async Task<VolumesListResponse> GetVolumes() {
    return await Client.Volumes.ListAsync(new VolumesListParameters());
  }
  #endregion
}