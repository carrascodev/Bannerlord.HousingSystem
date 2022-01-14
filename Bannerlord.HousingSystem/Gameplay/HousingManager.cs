using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.HousingSystem;

public class HousingManager : GenericSingleton<HousingManager>
{
    private string _path;

    public Dictionary<string,HouseConfig[]> Config { get; private set; }
    public HousingSettings Settings { get; private set; }
    
    public Dictionary<string, Dictionary<HouseTier, HouseData>> HouseInventory;
    private HouseData _currentData;


    public HousingManager()
    {
        _path = Path.Combine(BasePath.Name, "Modules", 
            "Bannerlord.HousingSystem", "Data", "Config.json");
        Load();
        HouseInventory = new Dictionary<string, Dictionary<HouseTier, HouseData>>();
    }
    
    public void Load()
    {
        string file = File.ReadAllText(_path);
        try
        {
            var json = JObject.Parse(file);
            Config = json["HouseConfigs"].ToObject<Dictionary<string,HouseConfig[]>>();
            Settings = json["Settings"].ToObject<HousingSettings>();
        }
        catch (JsonReaderException e)
        {
            Console.WriteLine(e);
        }
    }

    public int GetHousePriceForSettlement(Settlement settlement,HouseConfig config)
    {
        if (settlement.Prosperity >= Settings.GoodProsperity)
        {
            return config.Pricing[1];
        }
        else
        {
            return config.Pricing[0];
        }
    }

    public bool CanBuyHouse(Settlement settlement,HouseConfig config)
    {
        var settlementId = settlement.Id.ToString();
        if (!HouseInventory.ContainsKey(settlementId) || HouseInventory[settlementId] != null)
        {
            var pricing = GetHousePriceForSettlement(settlement,config);
            if (!IsHouseBought(settlement,config.Tier) && Hero.MainHero.Gold >= pricing)
                return true;
        }

        return false;
    }

    public void BuyHouse(Settlement settlement, HouseConfig config)
    {
        var settlementId = settlement.Id.ToString();
        if (CanBuyHouse(settlement, config))
        {
            var pricing = GetHousePriceForSettlement(settlement,config);
            HouseData data = new HouseData()
            {
                ConfigId = config.Id,
                ItemRoster = new ItemRoster(),
                PricePaid = pricing,
                StorageCapacity = config.StorageCapacity
                
            };
            if (!HouseInventory.ContainsKey(settlementId))
            {
                HouseInventory.Add(settlementId,new Dictionary<HouseTier, HouseData>());
            }
            
            HouseInventory[settlementId] ??= new Dictionary<HouseTier, HouseData>();
            HouseInventory[settlementId].Add(config.Tier,data);
            GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, pricing * -1);
            InformationManager.DisplayMessage(new InformationMessage($"You bought a {config.Name} for {pricing}", 
                Color.FromUint(Convert.ToUInt32("0xc4ff5e", 16))));
        }
    }

    public bool IsHouseBought(Settlement settlement, HouseTier tier)
    {
        var settlementId = settlement.Id.ToString();
        if (!HouseInventory.ContainsKey(settlementId) || HouseInventory[settlementId] == null)
        {
            return false;
        }
        return HouseInventory[settlementId].ContainsKey(tier);
    }

    public void OpenHouseRoster(Settlement settlement, HouseTier houseTier)
    {
        var settlementId = settlement.Id.ToString();
        if (!HouseInventory.ContainsKey(settlementId) || HouseInventory[settlementId] == null)
        {
            return;
        }
        _currentData = HouseInventory[settlementId][houseTier];
        _currentData.ItemRoster.RosterUpdatedEvent += OnItemRosterUpdated;
        InventoryManager.OpenScreenAsStash(_currentData.ItemRoster);
        InventoryManager.InventoryLogic.AfterTransfer += OnAfterTransfer;
    }

    private void OnAfterTransfer(InventoryLogic inventorylogic, List<TransferCommandResult> results)
    {
        InventoryManager.InventoryLogic.AfterTransfer -= OnAfterTransfer;
        _currentData.ItemRoster.RosterUpdatedEvent -= OnItemRosterUpdated;
    }

    private void OnItemRosterUpdated(ItemRosterElement item, int count)
    {
        if (_currentData.ItemRoster.Count > _currentData.StorageCapacity)
        {
            InventoryManager.InventoryLogic.TransferOne(item);
        }
    }

    public void RentHouse(Settlement settlement, HouseTier tier, bool rent = true)
    {
        var settlementId = settlement.Id.ToString();
        if (!HouseInventory.ContainsKey(settlementId) || HouseInventory[settlementId] == null)
        {
            return;
        }

        HouseInventory[settlementId][tier].IsRented = rent;
        int amountToEarn = (HouseInventory[settlementId][tier].PricePaid * Settings.RentRevenuePercentage) / 100;
        InformationManager.DisplayMessage(new InformationMessage($"You are now renting your property, predicted income: ${amountToEarn}",
            Color.FromUint(Convert.ToUInt32("0x73daff", 16))));
    }

    public int GetRentRevenue(Settlement settlement, HouseTier tier)
    {
        var settlementId = settlement.Id.ToString();
        if (!HouseInventory.ContainsKey(settlementId) || HouseInventory[settlementId] == null || !HouseInventory[settlementId][tier].IsRented)
        {
            return 0;
        }
        
        return HouseInventory[settlementId][tier].PricePaid / Settings.RentRevenuePercentage;
    }

    public bool IsHouseRented(Settlement settlement, HouseConfig config)
    {
        var settlementId = settlement.Id.ToString();
        if (!HouseInventory.ContainsKey(settlementId) || HouseInventory[settlementId] == null)
        {
            return false;
        }

        return HouseInventory[settlementId][config.Tier].IsRented;
    }
}