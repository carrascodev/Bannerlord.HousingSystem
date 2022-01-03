using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;

namespace Bannerlord.HousingSystem;

public class SubModule : MBSubModuleBase
{
    protected override void OnBeforeInitialModuleScreenSetAsRoot()
    {
        base.OnBeforeInitialModuleScreenSetAsRoot();
        InformationManager.DisplayMessage(new InformationMessage(
            "HousingSystem loaded!!",
            Color.FromUint(Convert.ToUInt32("0x92c3fb", 16))));
    }

    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
        base.OnGameStart(game, gameStarterObject);
        CampaignGameStarter campaignStarter = gameStarterObject as CampaignGameStarter;

        if (campaignStarter != null)
        {
            campaignStarter.AddBehavior(new HouseSystemBehaviour());
        }
        
    }
}