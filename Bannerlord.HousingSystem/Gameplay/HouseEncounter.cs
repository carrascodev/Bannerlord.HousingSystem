using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Bannerlord.HousingSystem;

public class HouseEncounter : TownEncounter
{
    public HouseEncounter(Settlement settlement) : base(settlement)
    {
    }

    public override IMission CreateAndOpenMissionController(Location nextLocation,
        Location previousLocation = null,
        CharacterObject talkToChar = null,
        string playerSpecialSpawnTag = null)
    {
        //TODO: custom loading here
        return base.CreateAndOpenMissionController(nextLocation, previousLocation, talkToChar, playerSpecialSpawnTag);
    }
}