using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Bannerlord.HousingSystem;

public class HousingManager : GenericSingleton<HousingManager>
{
    private string _path;

    public List<HouseConfig> Config { get; private set; }
    public HousingSettings Settings { get; private set; }
    
    public Dictionary<Settlement, HouseInventory> HouseInventory;

    
    public HousingManager()
    {
        _path = Path.Combine(BasePath.Name, "Modules", 
            "Bannerlord.HousingSystem", "Data", "Config.json");
        Load();
    }
    
    public void Load()
    {
        string file = File.ReadAllText(_path);
        try
        {
            var json = JObject.Parse(file);
            Config = json["HouseConfigs"].ToObject<List<HouseConfig>>();
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
        if (HouseInventory[settlement].Datas != null)
        {
            var pricing = GetHousePriceForSettlement(settlement,config);
            if (!IsHouseBought(settlement,config.Tier) && Hero.MainHero.Gold >= pricing)
                return true;
        }

        return false;
    }

    public void BuyHouse(Settlement settlement, HouseConfig config)
    {
        if (CanBuyHouse(settlement, config))
        {
            
        }
    }

    public bool IsHouseBought(Settlement settlement, HouseTier tier)
    {
        if (HouseInventory[settlement].Datas == null)
        {
            return false;
        }
        return HouseInventory[settlement].Datas.ContainsKey(tier);
    }
}