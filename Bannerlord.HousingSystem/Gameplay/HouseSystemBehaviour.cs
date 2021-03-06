using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.HousingSystem;

public class HouseSystemBehaviour : CampaignBehaviorBase
{
    private readonly HousingManager _housingManager = HousingManager.Instance;

    public override void RegisterEvents()
    {
        CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        CampaignEvents.WeeklyTickEvent.AddNonSerializedListener((object) this, OnWeeklyTick);
    }

    private void OnWeeklyTick()
    {
        if (_housingManager.HouseInventory == null)
        {
            return;
        }
        
        int amountToEarn = 0;
        foreach (Dictionary<HouseTier, HouseData> houseInventory in _housingManager.HouseInventory.Values)
        {
            var prices = houseInventory.Where(d => d.Value.IsRented)
                .Select(d => d.Value)
                .Sum(h => h.PricePaid);

            if (prices > 0)
            {
                amountToEarn += ((prices * _housingManager.Settings.RentRevenuePercentage) / 100);
            }
        }

        if (amountToEarn > 0)
        {
            GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, amountToEarn);
            InformationManager.DisplayMessage(new InformationMessage($"Weekly rent earnings added: ${amountToEarn}",
                Color.FromUint(Convert.ToUInt32("0x95ff85", 16))));
        }
    }

    private void OnSessionLaunched(CampaignGameStarter starter)
    {
        AddMenus(starter);
    }

    private void AddMenus(CampaignGameStarter starter)
    {
        starter.AddGameMenuOption("town", "town_housing", "{=!}Houses", CanOpenHousingMenu,
            (args) => OnOpenHousingMenuConsequence(args, starter), false, 3);

        Dictionary<string, HouseConfig[]> configs = _housingManager.Config;
        
        starter.AddGameMenu("housing_menu", "{=!}House Menu", _ =>
        {
            var cultureName = Settlement.CurrentSettlement.Culture.Name.ToString();
            if (configs.ContainsKey(cultureName))
            {
                var currentConfig = configs[Settlement.CurrentSettlement.Culture.Name.ToString()];
                foreach (var houseConfig in currentConfig)
                {
                    SetupOptionText(houseConfig);
                }    
            }
        });
        
        foreach (var cultureConfig in configs)
        {
            foreach (var config in cultureConfig.Value)
            {
                starter.AddGameMenuOption("housing_menu", config.Id, $"{{=!}}{{HOUSE_OPTION_{config.Id}}}",
                    args => CanBuyOrOpenHouseCondition(args, config),
                    args => OnBuyOrOpenHouseConsequence(args, config));
                starter.AddGameMenu(config.Id, "{=!}" + config.Name + " Menu", _ =>
                {
                    SetupSelectedHouseOptionText(config);
                }, GameOverlays.MenuOverlayType.SettlementWithBoth);
                starter.AddGameMenuOption(config.Id, $"{config.Id}_enter", "{=!}Enter in house", 
                    args => !_housingManager.IsHouseRented(Settlement.CurrentSettlement, config), 
                    args => OnHouseEnterConsequence(args, config));
                starter.AddGameMenuOption(config.Id, $"{config.Id}_rent", "{=!}{HOUSE_RENT_OPTION}", 
                    args => true, 
                    args => OnHouseRentConsequence(args, config));
                starter.AddGameMenuOption(config.Id, $"{config.Id}_open_storage", "{=!}Open Storage", 
                    args => !_housingManager.IsHouseRented(Settlement.CurrentSettlement, config), 
                    args => OnHouseOpenStorageConsequence(args, config));
                starter.AddGameMenuOption(config.Id, config.Id+"_go_back", "Go Back", _ => true, args =>
                {
                    GameMenu.SwitchToMenu("housing_menu");
                });   
            }
        }
        starter.AddGameMenuOption("housing_menu", "go_back", "Go Back", _ => true, args =>
        {
            GameMenu.SwitchToMenu("town");
        });
    }

    private bool CanOpenHousingMenu(MenuCallbackArgs args)
    {
        args.optionLeaveType = GameMenuOption.LeaveType.Manage;
        return CanAccess();
    }

    private void OnOpenHousingMenuConsequence(MenuCallbackArgs consequenceArgs, CampaignGameStarter starter)
    {
        GameMenu.SwitchToMenu("housing_menu");
    }

    private bool CanBuyOrOpenHouseCondition(MenuCallbackArgs args, HouseConfig config)
    {
        if (Settlement.CurrentSettlement.Culture.Name.ToString() != config.Culture)
        {
            return false;
        }
        
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
            MBTextManager.SetTextVariable("HOUSE_OPTION_" + config.Id, $"Manage your {config.Name}");
        }
        else
        {
            MBTextManager.SetTextVariable("HOUSE_OPTION_" + config.Id,
                $"Buy {config.Name} for {_housingManager.GetHousePriceForSettlement(Settlement.CurrentSettlement, config)}");
        }
    }

    private void SetupSelectedHouseOptionText(HouseConfig config)
    {
        if (!_housingManager.IsHouseRented(Settlement.CurrentSettlement, config))
        {
            MBTextManager.SetTextVariable("HOUSE_RENT_OPTION", $"Rent it out");
        }
        else
        {
            MBTextManager.SetTextVariable("HOUSE_RENT_OPTION", $"Stop renting");
        }
    }

    private void OnBuyOrOpenHouseConsequence(MenuCallbackArgs args, HouseConfig config)
    {
        if (Settlement.CurrentSettlement.Culture.Name.ToString() != config.Culture)
        {
            return;
        }
        
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

    private void OnHouseEnterConsequence(MenuCallbackArgs args, HouseConfig config)
    {
        var locationEncounter = new HouseEncounter(Settlement.CurrentSettlement,config.Id, config.SceneName);
        PlayerEncounter.LocationEncounter = locationEncounter;
        var mission = locationEncounter.CreateAndOpenMissionController(null) as Mission;
        //TODO:
    }

    private void OnHouseRentConsequence(MenuCallbackArgs args, HouseConfig config)
    {
        bool isRented = _housingManager.IsHouseRented(Settlement.CurrentSettlement, config); // true
        _housingManager.RentHouse(Settlement.CurrentSettlement, config.Tier, !isRented);
        GameMenu.SwitchToMenu(config.Id);
    }

    private void OnHouseOpenStorageConsequence(MenuCallbackArgs args, HouseConfig config)
    {
        HousingManager.Instance.OpenHouseRoster(Settlement.CurrentSettlement, config.Tier);
    }
    
    public override void SyncData(IDataStore dataStore)
    {
        dataStore.SyncData("_house_inventory", ref _housingManager.HouseInventory);
    }
}