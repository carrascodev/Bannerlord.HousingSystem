using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.HousingSystem;

public class HouseSystemBehaviour : CampaignBehaviorBase
{
    private readonly HousingManager _housingManager = HousingManager.Instance;

    public override void RegisterEvents()
    {
        _housingManager.HouseInventory = new Dictionary<Settlement, HouseInventory>();
        CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
    }

    private void OnSessionLaunched(CampaignGameStarter starter)
    {
        AddMenus(starter);
    }

    private void AddMenus(CampaignGameStarter starter)
    {
        starter.AddGameMenuOption("town", "town_housing", "{=!}Houses", CanOpenHousingMenu,
            (args) => OnOpenHousingMenuConsequence(args, starter));

        var configs = _housingManager.Config;
        
        starter.AddGameMenu("housing_menu", "{=!}House Menu", _ =>
        {
            foreach (var config in configs)
            {
                SetupOptionText(config);
            }
        });
        
        foreach (var config in configs)
        {
            starter.AddGameMenuOption("housing_menu", config.Id, $"{{=!}}{{HOUSE_OPTION_{config.Id}}}",
                args => CanBuyOrOpenHouseCondition(args, config),
                args => OnBuyOrOpenHouseConsequence(args, config));
            starter.AddGameMenu(config.Id, "{=!}" + config.Name + " Menu", _ => { }, GameOverlays.MenuOverlayType.SettlementWithBoth);
            starter.AddGameMenuOption(config.Id, $"{config.Id}_enter", "{=!}Enter in house", 
                args => true, 
                args => OnHouseEnterConsequence(args, config));
            starter.AddGameMenuOption(config.Id, $"{config.Id}_rent", "{=!}Rent", 
                args => true, 
                OnHouseRentConsequence);
            starter.AddGameMenuOption(config.Id, $"{config.Id}_open_storage", "{=!}Open Storage", 
                args => true, 
                args => OnHouseOpenStorageConsequence(args, config));
            starter.AddGameMenuOption(config.Id, config.Id+"_go_back", "Go Back", _ => true, args =>
            {
                GameMenu.SwitchToMenu("housing_menu");
            });
        }
        starter.AddGameMenuOption("housing_menu", "go_back", "Go Back", _ => true, args =>
        {
            GameMenu.SwitchToMenu("town");
        });
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
        if (_housingManager.IsHouseBought(Settlement.CurrentSettlement, config.Tier))
        {
            return true; // Can Open/Enter
        }

        return _housingManager.CanBuyHouse(Settlement.CurrentSettlement, config);
    }

    private void SetupOptionText(HouseConfig config)
    {
        if (_housingManager.IsHouseBought(Settlement.CurrentSettlement, config.Tier))
        {
            MBTextManager.SetTextVariable("HOUSE_OPTION_" + config.Id, $"Manager your {config.Name}");
        }
        else
        {
            MBTextManager.SetTextVariable("HOUSE_OPTION_" + config.Id,
                $"Buy {config.Name} for {_housingManager.GetHousePriceForSettlement(Settlement.CurrentSettlement, config)}");
        }
    }

    private void OnBuyOrOpenHouseConsequence(MenuCallbackArgs args, HouseConfig config)
    {
        if (!_housingManager.IsHouseBought(Settlement.CurrentSettlement, config.Tier))
        {
            _housingManager.BuyHouse(Settlement.CurrentSettlement, config);
        }

        GameMenu.SwitchToMenu(config.Id);
    }

    public bool CanAccess()
    {
        return Settlement.CurrentSettlement.IsTown;
    }

    public override void SyncData(IDataStore dataStore)
    {
        dataStore.SyncData("_house_inventory", ref _housingManager.HouseInventory);
    }

    private void OnHouseEnterConsequence(MenuCallbackArgs args, HouseConfig config)
    {
        var locationEncounter = new HouseEncounter(Settlement.CurrentSettlement);
        Campaign.Current.GameMenuManager.NextLocation = LocationComplex.Current.GetLocationWithId(config.Id);
        Campaign.Current.GameMenuManager.PreviousLocation = LocationComplex.Current.GetLocationWithId("center");
        locationEncounter.CreateAndOpenMissionController(Campaign.Current.GameMenuManager.NextLocation);
        Campaign.Current.GameMenuManager.NextLocation = null;
        Campaign.Current.GameMenuManager.PreviousLocation = null;
    }

    private void OnHouseRentConsequence(MenuCallbackArgs args)
    {
        //TODO:
    }

    private void OnHouseOpenStorageConsequence(MenuCallbackArgs args, HouseConfig config)
    {
        //TODO:
    }
}