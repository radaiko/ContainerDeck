using Agent.Helpers;
using ContainerDeck.Shared.Protos;
using ContainerDeck.Shared.Utils;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ContainerDeck.Agent.Services;

public class DockerVolumeService : DockerVolume.DockerVolumeBase {

  private readonly ILogger logger = Hub.GetLogger<DockerVolumeService>();

  public DockerVolumeService() {
    logger.LogDebug("DockerImageService created");
  }
  public override async Task<ProtoVolumesResponse> GetVolumes(Empty request, ServerCallContext context) {
    var volumes = await DockerWrapper.GetVolumes();
    var response = new ProtoVolumesResponse();
    foreach (var volume in volumes.Volumes) {
      if (volume == null) continue;

      var volumeUsage = new ProtoVolumeUsageData();

      if (volume.UsageData == null) {
        volumeUsage.RefCount = 0;
        volumeUsage.Size = 0;
      }
      else {
        volumeUsage.RefCount = volume.UsageData.RefCount;
        volumeUsage.Size = volume.UsageData.Size;
      }

      var volumeResponse = new ProtoVolumeResponse();
      volumeResponse.Name = volume.Name;
      volumeResponse.Created = DateTime.Parse(volume.CreatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind).ToProtoTimestamp();
      volumeResponse.Driver = volume.Driver;
      volumeResponse.Mountpoint = volume.Mountpoint; // Status = { volume.Status.ToDictionary(kvp => kvp.Key, kvp => Value.ForString(kvp.Value.ToString())).ToMapField() },
                                                     // Labels = { volume.Labels.ToMapField() }, 
                                                     // Options = { volume.Options.ToMapField() },
      volumeResponse.Scope = volume.Scope;
      volumeResponse.Usage = volumeUsage;
      response.Volumes.Add(volumeResponse);
    }
    return await Task.FromResult(response);
  }

}