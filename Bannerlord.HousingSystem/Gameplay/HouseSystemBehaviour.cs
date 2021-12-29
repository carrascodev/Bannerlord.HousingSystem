using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;

namespace Bannerlord.HousingSystem;

public class HouseSystemBehaviour : CampaignBehaviorBase
{
    public static Dictionary<Settlement, HouseInventory> HouseInventory;
    public override void RegisterEvents()
    {
        HouseInventory = new Dictionary<Settlement, HouseInventory>();
        CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddMenus);
    }

    private void AddMenus(CampaignGameStarter starter)
    {
        starter.AddGameMenuOption("town","town_housing", "{=town_housing}Houses", args =>
        {
            return CanAccess();
        }, OnOpenHousingMenuConsequence);
        starter.AddGameMenu("housing_menu","{=housing_menu_info}House Menu", _ =>
        {});
    }

    private void OnOpenHousingMenuConsequence(MenuCallbackArgs consequenceArgs)
    {
        GameMenu.SwitchToMenu("housing_menu");
        var configs = HousingManager.Instance.Config[Settlement.CurrentSettlement.StringId];
        foreach (var config in configs)
        {
            //starter.AddGameMenuOption();
        }
    }

    public bool CanAccess()
    {
        return Settlement.CurrentSettlement.IsTown;
    }

    public override void SyncData(IDataStore dataStore)
    {
        dataStore.SyncData("_house_inventory", ref HouseInventory);
    }
}