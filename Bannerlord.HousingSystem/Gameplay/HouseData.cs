using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Bannerlord.HousingSystem;

public class HouseData
{
    [SaveableField(1)]
    public string ConfigId;

    [SaveableField(2)]
    public int PricePaid;

    [SaveableField(3)]
    public ItemRoster ItemRoster;

    [SaveableField(4)]
    public int StorageCapacity;
    
    [SaveableField(5)]
    public bool IsRented;
}