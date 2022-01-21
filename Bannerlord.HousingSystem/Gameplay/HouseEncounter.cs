using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.HousingSystem;

public class HouseEncounter : TownEncounter
{
    private string _sceneName;
    private Location _location;
    public HouseEncounter(Settlement settlement,string id, string sceneName) : base(settlement)
    {
        _sceneName = sceneName;
        var text = new TextObject("{=!}House");
        _location = new Location(id, text, text, 5, true, true, 
            "CanIfSettlementAccessModelLetsPlayer", "CanIfSettlementAccessModelLetsPlayer", "CanNever", "CanNever", 
            new[] {sceneName, sceneName, sceneName, sceneName}, null);
    }

    void LoadLocation()
    {
        
    }

    public override IMission CreateAndOpenMissionController(Location nextLocation,
        Location previousLocation = null,
        CharacterObject talkToChar = null,
        string playerSpecialSpawnTag = null)
    {
        var missionController = CampaignMission.OpenIndoorMission(_sceneName, 0, _location, talkToChar) as Mission;
        return missionController;
    }

    public override bool IsTavern(Location location)
    {
        return true;
    }
}