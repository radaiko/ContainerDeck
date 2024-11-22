using ContainerDeck.Shared.Protos;

namespace ContainerDeck.Shared.Models;

#region class Volume -------------------------------------------------------------------------------
public class Volume
{
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public string Driver { get; set; }
    public string Mountpoint { get; set; }
    public Dictionary<string, string> Labels { get; set; }
    public Dictionary<string, string> Options { get; set; }
    public string Scope { get; set; }
    public Dictionary<string, object> Status { get; set; }
    public VolumeUsageData Usage { get; set; }

    public Volume(ProtoVolumeResponse protoVolume)
    {
        Name = protoVolume.Name;
        Created = protoVolume.Created.ToDateTime();
        Driver = protoVolume.Driver;
        Mountpoint = protoVolume.Mountpoint;
        Labels = protoVolume.Labels.ToDictionary();
        Options = protoVolume.Options.ToDictionary();
        Scope = protoVolume.Scope;
        Status = protoVolume.Status.ToDictionary(kvp => kvp.Key, object (kvp) => kvp.Value);
        Usage = new VolumeUsageData
        {
            RefCount = protoVolume.Usage.RefCount,
            Size = protoVolume.Usage.Size
        };

    }
    #region Nested -------------------------------------------------------------
    public class VolumeUsageData
    {
        public float RefCount { get; set; }
        public float Size { get; set; }
    }
    #endregion

}
#endregion


#region class Volumes ------------------------------------------------------------------------------
public class Volumes : List<Volume>
{
    public Volumes() { }
    public Volumes(IEnumerable<ProtoVolumeResponse> protoVolumes)
    {
        foreach (var protoVolume in protoVolumes)
        {
            Add(new Volume(protoVolume));
        }
    }

}
#endregion