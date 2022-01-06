using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Bannerlord.HousingSystem;

public class SaveDefiner : SaveableTypeDefiner
{
    public SaveDefiner() : base(4_420_421)
    {
    }

    protected override void DefineClassTypes()
    {
        AddEnumDefinition(typeof(HouseTier),1);
        AddClassDefinition(typeof(HouseData),2);
        AddClassDefinition(typeof(HouseInventory),3);
    }

    protected override void DefineContainerDefinitions()
    {
        ConstructContainerDefinition(typeof(Dictionary<HouseTier, HouseData>));
        ConstructContainerDefinition(typeof(Dictionary<string, Dictionary<HouseTier, HouseData>>));
    }
}