#region
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
#endregion

namespace ContainerDeck.Shared.Utils;

public static class IntExtensions
{
  public static TimeSpan ToTimeSpanFromSeconds(this int value)
  {
    if (value < 0)
    {
      Hub.LogWarning("ToTimeSpan() value was null or negative. Return 0");
      return TimeSpan.Zero;
    }
    return TimeSpan.FromSeconds(value);
  }
}

public static class StringExtensions
{
  /// <summary>
  ///   Converts a string into a int
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static int ToInt(this string value)
  {
    if (string.IsNullOrEmpty(value))
    {
      Hub.LogWarning("ToInt() value was null or empty. Return 0");
      return 0;
    }

    if (int.TryParse(value, out var result)) return result;

    Hub.LogWarning($"ToInt() was not able to parse {value}. Return 0");
    return 0;
  }

  /// <summary>
  ///   Converts a string to a uint
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static uint ToUInt(this string value)
  {
    if (string.IsNullOrEmpty(value))
    {
      Hub.LogWarning("ToUInt() value was null or empty. Return 0");
      return 0;
    }

    if (uint.TryParse(value, out var result)) return result;

    Hub.LogWarning($"ToUInt() was not able to parse {value}. Return 0");
    return 0;
  }

  /// <summary>
  ///   Converts a string to a double
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static double ToDouble(this string? value)
  {
    if (value.IsNullOrEmpty()) return 0;
    return Convert.ToDouble(value, CultureInfo.CurrentCulture);
  }

  public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);

  public static bool IsBlank(this string? value) => string.IsNullOrEmpty(value);


  /// <summary>Converts a seconds string into a TimeSpan</summary>
  public static TimeSpan ToTimeSpanFromSeconds(this string? value)
  {
    if (string.IsNullOrEmpty(value))
    {
      Hub.LogWarning("ToTimeSpan() value was null or empty. Return 0");
      return TimeSpan.Zero;
    }

    if (int.TryParse(value, out var seconds)) return TimeSpan.FromSeconds(seconds);

    Hub.LogWarning($"ToTimeSpan() was not able to parse {value}. Return 0");
    return TimeSpan.Zero;
  }

  /// <summary>Converts a hours based string like 2.7 to a TimeSpan</summary>
  public static TimeSpan ToTimeSpanFromHours(this string? value)
  {
    if (string.IsNullOrEmpty(value))
    {
      Hub.LogWarning("ToTimeSpanFromHours() value was null or empty. Return 0");
      return TimeSpan.Zero;
    }

    if (double.TryParse(value, out var hours)) return TimeSpan.FromHours(hours);

    Hub.LogWarning($"ToTimeSpanFromHours() was not able to parse {value}. Return 0");
    return TimeSpan.Zero;
  }

  /// <summary>Converts a string like 20221021 into a datetime</summary>
  public static DateTime ToDateTime(this string? value)
  {
    if (string.IsNullOrEmpty(value))
    {
      Hub.LogWarning("ToDateTime() value was null or empty. Return DateTime.MinValue");
      return DateTime.MinValue;
    }

    if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime)) return dateTime;

    Hub.LogWarning($"ToDateTime() was not able to parse {value}. Return DateTime.MinValue");
    return DateTime.MinValue;
  }

  public static bool ContainsAny(this string source, IEnumerable<string> target) => target.Any(source.Contains);

  public static bool ContainsAny(this string[] source, IEnumerable<string> target) => target.Any(source.Contains);

  public static bool ContainsAnySplitNewLine(this string source, string[] target) => source.Split("\n").ContainsAny(target);

  public static string CreateSpaceString(this int numberOfSpaces) => new(' ', numberOfSpaces);

  public static bool ContainsFormatIndependend(this string source, string target) => source.ToLower().Contains(target.ToLower()) ? true : false;

  public static string ToShortId(this string id)
  {
    if (id.StartsWith("sha256:")) id = id[7..];
    return id.Length > 12 ? id[..12] : id;
  }
}

public static class EnumExtensions
{
  public static T TryParseOrDefault<T>(this string input) where T : struct, IConvertible => !Enum.TryParse(input, true, out T result) ? (T)Enum.GetValues(typeof(T)).GetValue(0)! : result;
}

public static class DoubleExtensions
{
  public static int ToInt(this double input) => Convert.ToInt32(input);

  public static int RoundUpToInt(this double input) => (int)Math.Ceiling(input);

  public static double Round(this double input, int decimals) => Math.Round(input, decimals);

  public static TimeSpan ToTimeSpanFromMinutes(this double value)
  {
    if (value < 0)
    {
      Hub.LogWarning("ToTimeSpan() value was null or negative. Return 0");
      return TimeSpan.Zero;
    }
    return TimeSpan.FromMinutes(value);
  }

  public static decimal[] ToDecimalArray(this double[] input) => Array.ConvertAll(input, x => (decimal)x);
  public static decimal?[] ToDecimalArray(this double?[] input) => Array.ConvertAll(input, x => x == null ? (decimal?)null : (decimal)x);
  public static decimal?[] ToNullableDecimalArray(this double[] input) => Array.ConvertAll(input, x => (decimal?)x);

  public static (long Size, string Format) ToSizeTuple(this double size)
  {
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    double len = size;
    var order = 0;
    while (len >= 1024 && order < sizes.Length - 1)
    {
      order++;
      len = len / 1024;
    }

    return (Convert.ToInt64(len), sizes[order]);
  }

  public static string ToFileSize(this double size)
  {
    var (Size, Format) = size.ToSizeTuple();
    return $"{Size} {Format}";
  }
}

public static class HttpClientExtensions
{
  public static void AddJsonHeader(this HttpClient client)
  {
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
  }

  public static void DownloadFile(this HttpClient client, Uri url, string fileLocation)
  {
    var fileInfo = new FileInfo(fileLocation);
    var response = client.GetAsync(url).GetAwaiter().GetResult();
    response.EnsureSuccessStatusCode();
    using var ms = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
    using var fs = File.Create(fileInfo.FullName);
    ms.Seek(0, SeekOrigin.Begin);
    ms.CopyTo(fs);
  }

  public static string GetAsBase64String(this HttpClient client, Uri url) => Convert.ToBase64String(client.GetByteArrayAsync(url).GetAwaiter().GetResult());

  public static string GetString(this HttpClient client, Uri url) => client.GetStringAsync(url).GetAwaiter().GetResult();

  public static string? PutJson(this HttpClient client, Uri url, string body)
  {
    var content = new StringContent(body, Encoding.UTF8, "application/json");
    var response = client.PutAsync(url.ToString(), content).GetAwaiter().GetResult();
    if (response.IsSuccessStatusCode) return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    return null;
  }

  public static string? PostJson(this HttpClient client, Uri url, string body)
  {
    var content = new StringContent(body, Encoding.UTF8, "application/json");
    var response = client.PostAsync(url.ToString(), content).GetAwaiter().GetResult();
    if (response.IsSuccessStatusCode) return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

    Hub.LogError($"PostJson - {response.StatusCode} - {url} - {body}");
    return null;
  }

  public static bool Delete(this HttpClient client, Uri url)
  {
    var response = client.DeleteAsync(url).GetAwaiter().GetResult();
    return response.IsSuccessStatusCode;
  }
}

public static class DictionaryExtensions
{
  #region Public Methods
  public static int GetKeyByValue(this Dictionary<int, string> dict, string value)
  {
    return dict.FirstOrDefault(k => k.Value == value).Key;
  }

  public static Dictionary<TKey, TValue> MergeInPlace<TKey, TValue>(this Dictionary<TKey, TValue> left, Dictionary<TKey, TValue> right) where TKey : notnull
  {
    if (left == null)
      throw new ArgumentNullException(nameof(left));

    foreach (var kvp in right.Where(kvp => !left.ContainsKey(kvp.Key)))
      left.Add(kvp.Key, kvp.Value);

    return left;
  }

  public static void AddKeyValuePair(this Dictionary<string, string> dict, KeyValuePair<string, string> kv)
  {
    if (kv.Key == null || kv.Value == null) return;
    dict.Add(kv.Key, kv.Value);
  }
  #endregion Public Methods
}

public static class ListExtensions
{
  public static bool Contains<T>(this List<T> source, List<T> target)
  {
    if (target.Count == 0)
    {
      return true;
    }

    var targetIndex = 0;
    for (var i = 0; i < source.Count; i++)
    {
      if (source[i]!.Equals(target[targetIndex]))
      {
        targetIndex++;
        if (targetIndex == target.Count)
        {
          return true;
        }
      }
      else
      {
        targetIndex = 0;
      }
    }

    return false;
  }
  public static bool ContainsAny<T>(this List<T> source, List<T> target) => source.Any(target.Contains);
  public static bool HaveSameContent<T>(this List<T> first, List<T> second)
  {
    if (first == null || second == null)
      return false;

    return new HashSet<T>(first).SetEquals(second);
  }
}

public static class LongExtensions
{
  public static (long Size, string Format) ToSizeTuple(this long size)
  {
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    double len = size;
    var order = 0;
    while (len >= 1024 && order < sizes.Length - 1)
    {
      order++;
      len = len / 1024;
    }

    return (Convert.ToInt64(len), sizes[order]);
  }

  public static string ToFileSize(this long size)
  {
    var (Size, Format) = size.ToSizeTuple();
    return $"{Size} {Format}";
  }
}

public static class FileSystemInfoExtensions
{
  public static bool HasReadAccess(this FileSystemInfo fsi)
  {
    try
    {
      // Attempt to access the attributes of the file or directory
      var attributes = fsi.Attributes;
      if (fsi is DirectoryInfo directoryInfo)
      {
        directoryInfo.EnumerateFileSystemInfos();
      }
      return true; // If no exception is thrown, read access is available
    }
    catch (UnauthorizedAccessException)
    {
      Hub.LogWarning($"Unauthorized access to {fsi.FullName}");
      return false; // Read access is denied
    }
    catch (FileNotFoundException)
    {
      Hub.LogWarning($"File not found {fsi.FullName}");
      return false; // File or directory does not exist
    }
    catch (DirectoryNotFoundException)
    {
      Hub.LogWarning($"Directory not found {fsi.FullName}");
      return false; // Directory does not exist
    }
  }

  public static bool IsReparse(this FileSystemInfo fsi) => (fsi.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;

  public static bool IsOffline(this FileSystemInfo fsi) => (fsi.Attributes & FileAttributes.Offline) == FileAttributes.Offline;
}

public static class TimeSpanExtensions
{
  public static string ToReadableString(this TimeSpan span)
  {
    // if less than 1 minute return seconds
    // if less than 1 hour return minutes
    // if less than 1 day return hours
    // if less than 1 year return days
    // else return years
    if (span.TotalMinutes < 1) return $"{span.Seconds} s";
    if (span.TotalHours < 1) return $"{span.Minutes} m";
    if (span.TotalDays < 1) return $"{span.Hours} h";
    if (span.TotalDays < 365) return $"{span.Days} d";
    return $"{(span.Days / 365)} y";
  }
}

public static class ProtoMapperExtensions
{
  public static Google.Protobuf.WellKnownTypes.Timestamp ToProtoTimestamp(this DateTime dateTime)
  {
    return Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dateTime);
  }
  public static Google.Protobuf.Collections.MapField<TKey, TValue> ToMapField<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
  {
    var mapField = new Google.Protobuf.Collections.MapField<TKey, TValue>();
    foreach (var kvp in dictionary)
    {
      mapField[kvp.Key] = kvp.Value;
    }
    return mapField;
  }

}