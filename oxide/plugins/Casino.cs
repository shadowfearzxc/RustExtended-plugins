using System;
using System.Collections.Generic;
using Oxide.Core.Libraries;
using UnityEngine;
using RustExtended;
#pragma warning disable 0618 

namespace Oxide.Plugins
{
    [Info("Casino", "kustanovich", "1.0.3")]
    class Casino : RustLegacyPlugin	 
	{
			string chatName = "КАЗИНО";
			private System.Random getrandom;

			
						void Init() { UnityEngine.Debug.Log("[CASINO PLUGIN] RustExtended : Plugin casino is loaded");	 }
						void Loaded() {	getrandom = new System.Random(); }
			


		[ChatCommand("casino")]
		void casino(NetUser netuser, string command, string[] args)		
		{
                    rust.SendChatMessage(netuser, chatName, ("Добро пожаловать в казино, тут ты можешь все проиграть, а можешь и выиграть."));
					rust.SendChatMessage(netuser, chatName, ("Полагайся на свою удачу. Для входа в казино, используй команду [COLOR#388FFF]/casall"));
					rust.SendChatMessage(netuser, chatName, ("Минимальный баланс для входа : [COLOR#388FFF]5000"));
	
		}
		[ChatCommand("casall")]
		void casall(NetUser netuser, string command, string[] args)
		{
			
		ulong balance = Economy.GetBalance(netuser.userID);
		
		if(balance >= 5000) {
				int rnd=getrandom.Next(1, 9);
				if(rnd == 1) 
				{ 
				Economy.BalanceSub(netuser.userID, balance);
				rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы все проиграли ( [COLOR#388FFF]x0 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
				} 
				else if(rnd == 2) {
				Economy.BalanceSub(netuser.userID, 1000);
				rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли ( [COLOR#388FFF]1000 [COLOR#FFFAFA]). Вам баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								  }
				else if(rnd == 3) {
					Economy.BalanceAdd(netuser.userID, 1000); 
					rust.SendChatMessage(netuser, chatName, ("Поздравляем вы выиграли ( [COLOR#388FFF]1000 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
					}
				else if(rnd == 4) {
					Economy.BalanceAdd(netuser.userID, 5000);
					rust.SendChatMessage(netuser, chatName, ("Поздравляем вы выиграли ( [COLOR#388FFF]5000 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser, userID)));
								}
								else if (rnd == 5) {
									Economy.BalanceSub(netuser.userID, 5000) 
										rust.SendChatMessage(netuser, ChatName, ("Нам очень жаль! Вы проиграли ( [COLOR#388FFF] 5000 [COLOR#FFFAFA]). Ваш баланс : " + Economy.GetBalance(netuser, userID)));
									}
									else if (rnd == 6) {
									Economy.BalanceSub(netuser.userID, 10000) 
										rust.SendChatMessage(netuser, ChatName, ("Нам очень жаль! Вы проиграли ( [COLOR#388FFF] 10000 [COLOR#FFFAFA]). Ваш баланс : " + Economy.GetBalance(netuser, userID)));
									
									}
									else if (rnd == 7) {
										
									Economy.BalanceAdd(netuser.userID, 10000) 
										rust.SendChatMessage(netuser, ChatName, ("Поздравляем вы выиграли ( [COLOR#388FFF] 5000 [COLOR#FFFAFA]). Ваш баланс : " + Economy.GetBalance(netuser, userID)));
									}
									else if (rnd == 8) 
										{
									Economy.BalanceAdd(netuser.userID, 50000) 
										rust.SendChatMessage(netuser, ChatName, ("Поздравляем вы выиграли ( [COLOR#388FFF] 50000 [COLOR#FFFAFA]). Ваш баланс : " + Economy.GetBalance(netuser, userID)));
									}
									else if (rnd == 9) 
										{ 
				Economy.BalanceSub(netuser.userID, balance);
				rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы все проиграли ( [COLOR#388FFF]x0 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
				} 
										
		}
									
									
		else {rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в казино, потому что баланс меньше : [COLOR#388FFF]5000"));}
		
		

	
					
					
					
		
		
		
		
		
		
		void cmdSqlUpdate()
        {
            foreach (PlayerClient player in PlayerClient.All)
            {
			   var controllable = player.controllable;
			   var user = controllable.netUser;
			   OnSqlEcon(user);
            }
        }
		void OnSqlEcon(NetUser player)
		{
			
		}
	}
}