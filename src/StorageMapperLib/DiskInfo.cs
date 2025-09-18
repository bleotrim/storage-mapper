using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StorageMapperLib;

public class DiskInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("serial")]
    public string? Serial { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    [JsonPropertyName("partuuid")]
    public string? PartUuid { get; set; }

    [JsonPropertyName("mountpoint")]
    public string? Mountpoint { get; set; }

    [JsonPropertyName("fstype")]
    public string? Fstype { get; set; }

    [JsonPropertyName("children")]
    public List<DiskInfo>? Children { get; set; }
}

public class LsblkResult
{
    [JsonPropertyName("blockdevices")]
    public List<DiskInfo> DiskInfo { get; set; }
}