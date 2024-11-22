// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/Docker/docker_system.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ContainerDeck.Base.Protos
{

  /// <summary>Holder for reflection information generated from Protos/Docker/docker_system.proto</summary>
  public static partial class DockerSystemReflection
  {

    #region Descriptor
    /// <summary>File descriptor for Protos/Docker/docker_system.proto</summary>
    public static pbr::FileDescriptor Descriptor
    {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static DockerSystemReflection()
    {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiFQcm90b3MvRG9ja2VyL2RvY2tlcl9zeXN0ZW0ucHJvdG8SBkRvY2tlchob",
            "Z29vZ2xlL3Byb3RvYnVmL2VtcHR5LnByb3RvIikKD1ZlcnNpb25SZXNwb25z",
            "ZRIWCg52ZXJzaW9uX3N0cmluZxgBIAEoCTJNCgxEb2NrZXJTeXN0ZW0SPQoK",
            "R2V0VmVyc2lvbhIWLmdvb2dsZS5wcm90b2J1Zi5FbXB0eRoXLkRvY2tlci5W",
            "ZXJzaW9uUmVzcG9uc2VCGqoCF0RvY2tlclBpbG90LkJhc2UuUHJvdG9zYgZw",
            "cm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.WellKnownTypes.EmptyReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ContainerDeck.Base.Protos.VersionResponse), global::ContainerDeck.Base.Protos.VersionResponse.Parser, new[]{ "VersionString" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class VersionResponse : pb::IMessage<VersionResponse>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
#endif
  {
    private static readonly pb::MessageParser<VersionResponse> _parser = new pb::MessageParser<VersionResponse>(() => new VersionResponse());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<VersionResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor
    {
      get { return global::ContainerDeck.Base.Protos.DockerSystemReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor
    {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public VersionResponse()
    {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public VersionResponse(VersionResponse other) : this()
    {
      versionString_ = other.versionString_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public VersionResponse Clone()
    {
      return new VersionResponse(this);
    }

    /// <summary>Field number for the "version_string" field.</summary>
    public const int VersionStringFieldNumber = 1;
    private string versionString_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string VersionString
    {
      get { return versionString_; }
      set
      {
        versionString_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other)
    {
      return Equals(other as VersionResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(VersionResponse other)
    {
      if (ReferenceEquals(other, null))
      {
        return false;
      }
      if (ReferenceEquals(other, this))
      {
        return true;
      }
      if (VersionString != other.VersionString) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hash = 1;
      if (VersionString.Length != 0) hash ^= VersionString.GetHashCode();
      if (_unknownFields != null)
      {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString()
    {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
#else
      if (VersionString.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(VersionString);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output)
    {
      if (VersionString.Length != 0)
      {
        output.WriteRawTag(10);
        output.WriteString(VersionString);
      }
      if (_unknownFields != null)
      {
        _unknownFields.WriteTo(ref output);
      }
    }
#endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int size = 0;
      if (VersionString.Length != 0)
      {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(VersionString);
      }
      if (_unknownFields != null)
      {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(VersionResponse other)
    {
      if (other == null)
      {
        return;
      }
      if (other.VersionString.Length != 0)
      {
        VersionString = other.VersionString;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
#else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
      if ((tag & 7) == 4) {
        // Abort on any end group tag.
        return;
      }
      switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            VersionString = input.ReadString();
            break;
          }
        }
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input)
    {
      uint tag;
      while ((tag = input.ReadTag()) != 0)
      {
        if ((tag & 7) == 4)
        {
          // Abort on any end group tag.
          return;
        }
        switch (tag)
        {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10:
            {
              VersionString = input.ReadString();
              break;
            }
        }
      }
    }
#endif

  }

  #endregion

}

#endregion Designer generated code