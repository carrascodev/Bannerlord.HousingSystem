using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace Bannerlord.HousingSystem;

public class HouseInventory
{
    [SaveableField(1)]
    public Dictionary<HouseTier, HouseData> Datas;
}