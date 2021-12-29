using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaleWorlds.Library;

namespace Bannerlord.HousingSystem;

public class HousingManager : GenericSingleton<HousingManager>
{
    private string _path;

    public Dictionary<string,List<HouseConfig>> Config { get; private set; }
    
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
            Config = json.ToObject<Dictionary<string,List<HouseConfig>>>();
        }
        catch (JsonReaderException e)
        {
            Console.WriteLine(e);
        }
    }
}