using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Bannerlord.HousingSystem;

public class SaveDefiner : SaveableTypeDefiner
{
    public SaveDefiner() : base(487420007)
    {
    }

    protected override void DefineClassTypes()
    {
        AddClassDefinition(typeof(HouseData),1);
        AddClassDefinition(typeof(HouseInventory),2);
    }

    protected override void DefineContainerDefinitions()
    {
        ConstructContainerDefinition(typeof(Dictionary<Settlement, HouseInventory>));
    }
}