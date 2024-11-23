using ContainerDeck.Shared;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ContainerDeck.Agent.Services;

public class HealthService : Health.HealthBase {
  public override Task<HealthResponse> GetHealth(Empty request, ServerCallContext context) => Task.FromResult(new HealthResponse() { Status = HealthStatus.Healthy });
}