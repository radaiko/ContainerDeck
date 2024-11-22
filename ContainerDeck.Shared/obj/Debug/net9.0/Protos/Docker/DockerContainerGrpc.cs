// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/Docker/docker_container.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace ContainerDeck.Base.Protos
{
  public static partial class DockerContainer
  {
    static readonly string __ServiceName = "Docker.DockerContainer";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
#if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
#endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
#if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
#endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Google.Protobuf.WellKnownTypes.Empty> __Marshaller_google_protobuf_Empty = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Google.Protobuf.WellKnownTypes.Empty.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::ContainerDeck.Base.Protos.ProtoContainersResponse> __Marshaller_Docker_ProtoContainersResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ContainerDeck.Base.Protos.ProtoContainersResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::ContainerDeck.Base.Protos.ProtoContainerRequest> __Marshaller_Docker_ProtoContainerRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ContainerDeck.Base.Protos.ProtoContainerRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::ContainerDeck.Base.Protos.ProtoContainerResponse> __Marshaller_Docker_ProtoContainerResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ContainerDeck.Base.Protos.ProtoContainerResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest> __Marshaller_Docker_ProtoContainerInspectRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse> __Marshaller_Docker_ProtoContainerInspectResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::ContainerDeck.Base.Protos.ProtoContainerLogRequest> __Marshaller_Docker_ProtoContainerLogRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ContainerDeck.Base.Protos.ProtoContainerLogRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::ContainerDeck.Base.Protos.ProtoContainerLogResponse> __Marshaller_Docker_ProtoContainerLogResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ContainerDeck.Base.Protos.ProtoContainerLogResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::ContainerDeck.Base.Protos.ProtoContainersResponse> __Method_GetContainers = new grpc::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::ContainerDeck.Base.Protos.ProtoContainersResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetContainers",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_Docker_ProtoContainersResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::ContainerDeck.Base.Protos.ProtoContainerRequest, global::ContainerDeck.Base.Protos.ProtoContainerResponse> __Method_GetContainer = new grpc::Method<global::ContainerDeck.Base.Protos.ProtoContainerRequest, global::ContainerDeck.Base.Protos.ProtoContainerResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetContainer",
        __Marshaller_Docker_ProtoContainerRequest,
        __Marshaller_Docker_ProtoContainerResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest, global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse> __Method_GetContainerInspect = new grpc::Method<global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest, global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetContainerInspect",
        __Marshaller_Docker_ProtoContainerInspectRequest,
        __Marshaller_Docker_ProtoContainerInspectResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::ContainerDeck.Base.Protos.ProtoContainerLogRequest, global::ContainerDeck.Base.Protos.ProtoContainerLogResponse> __Method_StreamLog = new grpc::Method<global::ContainerDeck.Base.Protos.ProtoContainerLogRequest, global::ContainerDeck.Base.Protos.ProtoContainerLogResponse>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "StreamLog",
        __Marshaller_Docker_ProtoContainerLogRequest,
        __Marshaller_Docker_ProtoContainerLogResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::ContainerDeck.Base.Protos.DockerContainerReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of DockerContainer</summary>
    [grpc::BindServiceMethod(typeof(DockerContainer), "BindService")]
    public abstract partial class DockerContainerBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::ContainerDeck.Base.Protos.ProtoContainersResponse> GetContainers(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::ContainerDeck.Base.Protos.ProtoContainerResponse> GetContainer(global::ContainerDeck.Base.Protos.ProtoContainerRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse> GetContainerInspect(global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task StreamLog(global::ContainerDeck.Base.Protos.ProtoContainerLogRequest request, grpc::IServerStreamWriter<global::ContainerDeck.Base.Protos.ProtoContainerLogResponse> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for DockerContainer</summary>
    public partial class DockerContainerClient : grpc::ClientBase<DockerContainerClient>
    {
      /// <summary>Creates a new client for DockerContainer</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public DockerContainerClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for DockerContainer that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public DockerContainerClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected DockerContainerClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected DockerContainerClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::ContainerDeck.Base.Protos.ProtoContainersResponse GetContainers(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetContainers(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::ContainerDeck.Base.Protos.ProtoContainersResponse GetContainers(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetContainers, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::ContainerDeck.Base.Protos.ProtoContainersResponse> GetContainersAsync(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetContainersAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::ContainerDeck.Base.Protos.ProtoContainersResponse> GetContainersAsync(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetContainers, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::ContainerDeck.Base.Protos.ProtoContainerResponse GetContainer(global::ContainerDeck.Base.Protos.ProtoContainerRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetContainer(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::ContainerDeck.Base.Protos.ProtoContainerResponse GetContainer(global::ContainerDeck.Base.Protos.ProtoContainerRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetContainer, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::ContainerDeck.Base.Protos.ProtoContainerResponse> GetContainerAsync(global::ContainerDeck.Base.Protos.ProtoContainerRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetContainerAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::ContainerDeck.Base.Protos.ProtoContainerResponse> GetContainerAsync(global::ContainerDeck.Base.Protos.ProtoContainerRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetContainer, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse GetContainerInspect(global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetContainerInspect(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse GetContainerInspect(global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetContainerInspect, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse> GetContainerInspectAsync(global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetContainerInspectAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse> GetContainerInspectAsync(global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetContainerInspect, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncServerStreamingCall<global::ContainerDeck.Base.Protos.ProtoContainerLogResponse> StreamLog(global::ContainerDeck.Base.Protos.ProtoContainerLogRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return StreamLog(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncServerStreamingCall<global::ContainerDeck.Base.Protos.ProtoContainerLogResponse> StreamLog(global::ContainerDeck.Base.Protos.ProtoContainerLogRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_StreamLog, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override DockerContainerClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new DockerContainerClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(DockerContainerBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_GetContainers, serviceImpl.GetContainers)
          .AddMethod(__Method_GetContainer, serviceImpl.GetContainer)
          .AddMethod(__Method_GetContainerInspect, serviceImpl.GetContainerInspect)
          .AddMethod(__Method_StreamLog, serviceImpl.StreamLog).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, DockerContainerBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_GetContainers, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Google.Protobuf.WellKnownTypes.Empty, global::ContainerDeck.Base.Protos.ProtoContainersResponse>(serviceImpl.GetContainers));
      serviceBinder.AddMethod(__Method_GetContainer, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ContainerDeck.Base.Protos.ProtoContainerRequest, global::ContainerDeck.Base.Protos.ProtoContainerResponse>(serviceImpl.GetContainer));
      serviceBinder.AddMethod(__Method_GetContainerInspect, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ContainerDeck.Base.Protos.ProtoContainerInspectRequest, global::ContainerDeck.Base.Protos.ProtoContainerInspectResponse>(serviceImpl.GetContainerInspect));
      serviceBinder.AddMethod(__Method_StreamLog, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::ContainerDeck.Base.Protos.ProtoContainerLogRequest, global::ContainerDeck.Base.Protos.ProtoContainerLogResponse>(serviceImpl.StreamLog));
    }

  }
}
#endregion