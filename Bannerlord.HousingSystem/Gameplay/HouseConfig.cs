using MessagePack;

namespace Bannerlord.HousingSystem;

[MessagePackObject]
public class HouseConfig
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string SceneName { get; set; }
    public int[] Pricing { get; set; }
    public string SettlementId { get; set; }
    public int StorageCapacity { get; set; }
    public HouseTier Tier { get; set; }
}