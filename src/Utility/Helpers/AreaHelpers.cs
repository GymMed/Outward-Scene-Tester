using SceneTester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardSceneTester.Utility.Helpers
{
    public static class AreaHelpers
    {
        public static void TravelTo(AreaManager.AreaEnum area)
        {
            if(area == AreaManager.AreaEnum.HallowedDungeon4)
            {
                ST.LogMessage("Trying to load Hallowed_Dungeon4 (Dark Ziggurat), which doesn't work! You should use Hallowed_Dungeon4_Interior (Dark Ziggurat Interior) instead!");
            }

            TravelData travelData = new TravelData();

            travelData.RationCost = 0;
            travelData.SilverCost = 0;
            travelData.From = GetAreaEnumFromArea(AreaManager.Instance.CurrentArea);
            travelData.Destination = area;

            ProceedFastTravel(Global.Lobby.PlayersInLobby[0].ControlledCharacter, travelData);
        }

        public static void ProceedFastTravel(Character _instigator, TravelData _travelData)
        {
            if (NetworkLevelLoader.Instance.GetAllPlayerCanTravel() && GetAllPlayersOwnDLC(_instigator, true))
            {
                CharacterManager.Instance.SendMerchantFastTravel(_instigator, _travelData);
            }
        }

        public static bool GetAllPlayersOwnDLC(Character _instigator, bool _notifyOnFail = false)
        {
            bool allPlayersOwnDLC = CharacterManager.Instance.GetAllPlayersOwnDLC(OTWStoreAPI.DLCs.Soroboreans);
            if (!allPlayersOwnDLC && _notifyOnFail && _instigator && _instigator.CharacterUI)
            {
                _instigator.CharacterUI.ShowInfoNotificationLoc("Notification_Interaction_DLCRequiredCOOP", new string[]
                {
                    StoreManager.Instance.GetDLCName(OTWStoreAPI.DLCs.Soroboreans)
                });
            }
            return allPlayersOwnDLC;
        }

        public static AreaManager.AreaEnum GetAreaEnumFromArea(Area area)
        {
            return (AreaManager.AreaEnum)area.ID;
        }
    }
}
