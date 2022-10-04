using System;
using System.Collections.Generic;
using Oxide.Core;
//using Oxide.Core.Libraries;
using RustExtended;
#pragma warning disable 0618 

namespace Oxide.Plugins
{
    [Info("Top", "kustanovich", "2.1.2")]
    class Top : RustLegacyPlugin	 
	{
		string ChatName = RustExtended.Core.ServerName;

		[ChatCommand("top")]
		void top(NetUser netuser, string command, string[] args)		
		{
            rust.SendChatMessage(netuser, ChatName, "ТОП ИГРОКОВ СЕРВЕРА : ");
            List<UserData> topList = new List<UserData>();
            List<String> colors = new List<String>();
            colors.Add("[COLOR # 388FFF]");
            colors.Add("[COLOR # 388FFF]");
            colors.Add("[COLOR # 388FFF]");
            colors.Add("[COLOR # 388FFF]");
            colors.Add("[COLOR # 388FFF]");
			colors.Add("[COLOR # 388FFF]");
			colors.Add("[COLOR # 388FFF]");			 
			colors.Add("[COLOR # 388FFF]");
			colors.Add("[COLOR # 388FFF]");		
			colors.Add("[COLOR # 388FFF]");			
            for (int n = 1; n <= 10; n++)
	
            {
                UserData topPlayer = null;
                bool flag = false;
                float maxKD = 0;
                foreach (UserData userData in Users.All)
                {
                    float thisKD = (RustExtended.Economy.Get(userData.SteamID).PlayersKilled);
                    if ((!flag || thisKD > maxKD) && !topList.Contains(userData))
                    {
                        flag = true;
                        maxKD = thisKD;
                        topPlayer = userData;
                    }
                }
                topList.Add(topPlayer);
                float kills = RustExtended.Economy.Get(topPlayer.SteamID).PlayersKilled;
                float deaths = RustExtended.Economy.Get(topPlayer.SteamID).Deaths;
                rust.SendChatMessage(netuser, ChatName, $"[color#ffffff]{n}.  {colors[n - 1]}{topPlayer.Username}[COLOR#FFFAFA] - [COLOR#388FFF]{kills} [COLOR#FFFAFA]убийств, [COLOR#388FFF]{deaths} [COLOR#FFFAFA]смертей");
            }
        }
		static void SendMessage(PlayerClient player, string message) { ConsoleNetworker.SendClientCommand(player.netPlayer, "chat.add REBORN "+ Facepunch.Utility.String.QuoteSafe(message));  }		
			
			[ChatCommand("stat")]
		void statistic(NetUser netuser, string command, string[] args)		
		{			
			SendMessage(netuser.playerClient, string.Format("{0}", "Твоя личная статистика на сервере : "));
			UserData userData = Users.GetBySteamID(netuser.userID);
			string plkil = "Убийств: [COLOR#388FFF] "+RustExtended.Economy.Get(netuser.userID).PlayersKilled+"[color#FFFF00]";
			string death = "Смертей: [COLOR#388FFF] "+RustExtended.Economy.Get(netuser.userID).Deaths + "[color#FFFF00]";
			string kdeaths = "КД: [COLOR#388FFF] " + RustExtended.Economy.Get(netuser.userID).PlayersKilled/RustExtended.Economy.Get(netuser.userID).Deaths + "[color#ffff00]";

			SendMessage(netuser.playerClient, string.Format("{0}", plkil));
			SendMessage(netuser.playerClient, string.Format("{0}", death));		
SendMessage(netuser.playerClient, string.Format("{0}", kdeaths));				
		}
	}
}