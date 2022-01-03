using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bannerlord.HousingSystem;

[JsonObject]
public class HouseConfig
{
    [JsonProperty]
    public string Id { get; set; }
    
    [JsonProperty]
    public string Name { get; set; }
    [JsonProperty]
    public string SceneName { get; set; }
    [JsonProperty]
    public int[] Pricing { get; set; }
    [JsonProperty]
    public string SettlementId { get; set; }
    [JsonProperty]
    public int StorageCapacity { get; set; }
    [JsonProperty, JsonConverter(typeof(StringEnumConverter))]
    public HouseTier Tier { get; set; }
}