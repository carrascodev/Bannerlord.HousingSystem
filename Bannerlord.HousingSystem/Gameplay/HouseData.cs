using MessagePack;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Bannerlord.HousingSystem;

public class HouseData
{
    [SaveableProperty(1)]
    public string ConfigId { get; set; }
    [SaveableProperty(2)]
    public int PricePaid { get; set; }
    [SaveableProperty(3)]
    public ItemRoster ItemRoster { get; set; }
    [SaveableProperty(4)]
    public int StorageCapacity { get; set; }

    [SaveableProperty(5)]
    public bool IsRented { get; set; }
}