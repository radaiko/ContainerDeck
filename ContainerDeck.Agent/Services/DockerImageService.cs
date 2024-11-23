using Agent.Helpers;
using ContainerDeck.Shared.Protos;
using ContainerDeck.Shared.Utils;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ContainerDeck.Agent.Services;

public class DockerImageService : DockerImage.DockerImageBase {

  private readonly ILogger logger = Hub.GetLogger<DockerImageService>();

  public DockerImageService() {
    logger.LogDebug("DockerImageService created");
  }

  public override async Task<ProtoImagesResponse> GetImages(Empty request, ServerCallContext context) {
    var images = await DockerWrapper.GetImages();
    var response = new ProtoImagesResponse();
    foreach (var image in images) {
      if (image == null) continue;
      var imageDataResponse = new ProtoImageResponse {
        Id = image.ID,
        Name = image.RepoTags[0].Split(':')[0],
        Tag = image.RepoTags[0].Split(':')[1],
        IsUsed = false, // TODO: add logic to check if image is used
        Created = image.Created.ToProtoTimestamp(),
        Size = image.Size,
        IsUpdateAvailable = false //TODO: add logic to check if image is outdated
      };
      response.Images.Add(imageDataResponse);
    }
    return await Task.FromResult(response);
  }

  public override async Task<ProtoImageResponse> GetImage(ProtoImageRequest request, ServerCallContext context) {
    var image = await DockerWrapper.GetImage(request.Id);
    if (image == null) return await Task.FromResult(new ProtoImageResponse());
    var response = new ProtoImageResponse {
      Id = image.ID,
      Name = image.RepoTags[0].Split(':')[0],
      Tag = image.RepoTags[0].Split(':')[1],
      IsUsed = false, // TODO: add logic to check if image is used
      Created = image.Created.ToProtoTimestamp(),
      Size = image.Size,
      IsUpdateAvailable = false //TODO: add logic to check if image is outdated
    };
    return await Task.FromResult(response);
  }

}