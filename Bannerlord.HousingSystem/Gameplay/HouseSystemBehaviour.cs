using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;

namespace Bannerlord.HousingSystem;

public class HouseSystemBehaviour : CampaignBehaviorBase
{
    public override void RegisterEvents()
    {
        HousingManager.Instance.HouseInventory = new Dictionary<Settlement, HouseInventory>();
        CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
    }

    private void OnSessionLaunched(CampaignGameStarter starter)
    {
        AddMenus(starter);
    }

    private void AddMenus(CampaignGameStarter starter)
    {
        starter.AddGameMenuOption("town","town_housing", "{=!}Houses", CanOpenHousingMenu, 
            (args) => OnOpenHousingMenuConsequence(args, starter));
        
        var configs = HousingManager.Instance.Config;
        foreach (var config in configs)
        {
            starter.AddGameMenuOption("housing_menu", config.Id, $"{{=!}}{{OPTION_{config.Id}}}",
                args => CanBuyOrOpenHouseCondition(args, config), 
                args => OnBuyOrOpenHouseConsequence(args, config));
            starter.AddGameMenu(config.Id, "{=!}"+config.Name+" Menu", _ => {});
        }
        starter.AddGameMenu("housing_menu","{=!}House Menu", _ => {});
    }

    private bool CanOpenHousingMenu(MenuCallbackArgs args)
    {
        return CanAccess();
    }
    
    private void OnOpenHousingMenuConsequence(MenuCallbackArgs consequenceArgs, CampaignGameStarter starter)
    {
        GameMenu.SwitchToMenu("housing_menu");
    }

    private bool CanBuyOrOpenHouseCondition(MenuCallbackArgs args, HouseConfig config)
    {
        if (HousingManager.Instance.IsHouseBought(Settlement.CurrentSettlement, config.Tier))
        {
            return true; // Can Open/Enter
        }

        return HousingManager.Instance.CanBuyHouse(Settlement.CurrentSettlement, config);
    }

    private void OnBuyOrOpenHouseConsequence(MenuCallbackArgs args, HouseConfig config)
    {
        if (HousingManager.Instance.IsHouseBought(Settlement.CurrentSettlement, config.Tier))
        {
            GameMenu.SwitchToMenu(config.Id);
            return;
        }

        HousingManager.Instance.BuyHouse(Settlement.CurrentSettlement, config);
    }

    public bool CanAccess()
    {
        return Settlement.CurrentSettlement.IsTown;
    }

    public override void SyncData(IDataStore dataStore)
    {
        dataStore.SyncData("_house_inventory", ref HousingManager.Instance.HouseInventory);
    }
}