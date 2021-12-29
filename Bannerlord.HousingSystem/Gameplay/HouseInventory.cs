using System.Collections.Generic;
using MessagePack;
using TaleWorlds.SaveSystem;

namespace Bannerlord.HousingSystem;

public class HouseInventory
{
    [SaveableProperty(1)]
    public Dictionary<HouseTier, HouseData> Datas { get; set; }
}