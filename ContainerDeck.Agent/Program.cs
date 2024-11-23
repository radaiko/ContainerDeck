using Agent.Services;
using ContainerDeck.Agent.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpsRedirection(options => {
    options.HttpsPort = 5001;
});


// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseHttpsRedirection();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<LogService>();
app.MapGrpcService<DockerSystemService>();
app.MapGrpcService<HealthService>();
app.MapGrpcService<DockerImageService>();
app.MapGrpcService<DockerContainerService>();
app.MapGrpcService<DockerVolumeService>();


app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();
