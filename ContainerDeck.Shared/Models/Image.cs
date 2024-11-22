using System.Text.Json.Serialization;
using ContainerDeck.Shared.Utils;
using ContainerDeck.Shared.Protos;

namespace ContainerDeck.Shared.Models;

#region class Image --------------------------------------------------------------------------------
public class Image
{
    #region Properties ---------------------------------------------------------
    public string Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public ImageStatus Status { get; set; }
    public DateTime Created { get; set; }
    public long Size { get; set; }
    public bool IsUpdateAvailable { get; set; }
    // Computed properties
    public TimeSpan Age() => DateTime.Now.ToUniversalTime() - Created;
    public string AgeFormatted() => Age().ToReadableString();
    public string SizeFormatted() => Size.ToFileSize();
    #endregion

    #region Constructor --------------------------------------------------------
    public Image(ProtoImageResponse protoImage)
    {
        Id = protoImage.Id;
        Name = protoImage.Name;
        Tag = protoImage.Tag;
        Status = protoImage.IsUsed ? ImageStatus.InUse : ImageStatus.Unused;
        Created = protoImage.Created.ToDateTime();
        Size = protoImage.Size;
        IsUpdateAvailable = protoImage.IsUpdateAvailable;
    }
    #endregion
}
#endregion

#region class Images -------------------------------------------------------------------------------
public class Images
{
    #region Properties and Events ----------------------------------------------
    public List<Image> GetAll() => _images.OrderBy(i => i.Name).ToList();
    public int Count() => _images.Count;
    public string TotalSize() => _images.Sum(i => i.Size).ToFileSize();
    #endregion

    #region Constructor and private fields -------------------------------------
    [JsonInclude]
    private List<Image> _images { get; set; } = [];

    #endregion

    #region Public Methods -----------------------------------------------------
    public void Clear()
    {
        _images.Clear();
    }
    public void SetImages(List<Image> images)
    {
        _images.Clear();
        _images.AddRange(images);
    }
    #endregion
}
#endregion

#region enum ImageStatus ---------------------------------------------------------------------------
public enum ImageStatus
{
    InUse,
    Unused
}
#endregion
