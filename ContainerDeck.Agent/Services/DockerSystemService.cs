using Agent.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ContainerDeck.Shared.Protos;
using ContainerDeck.Shared.Utils;

namespace Agent.Services {
  public class DockerSystemService : DockerSystem.DockerSystemBase {

    private readonly ILogger logger = Hub.GetLogger<DockerSystemService>();

    public DockerSystemService() {
      logger.LogDebug("DockerSystemService created");
    }

    public override async Task<VersionResponse> GetVersion(Empty request, ServerCallContext context) {
      var dVersion = await DockerWrapper.GetVersion();
      var pVersion = new VersionResponse { VersionString = dVersion.Version };
      logger.LogDebug($"Docker version: {dVersion.Version}");
      return await Task.FromResult(pVersion);
    }
  }
}