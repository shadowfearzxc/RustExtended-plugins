using System;
using System.Collections.Generic;
using Oxide.Core.Libraries;
using UnityEngine;
using RustExtended;
#pragma warning disable 0618 

namespace Oxide.Plugins
{
    [Info("Casino", "kustanovich", 1.0)]
    class Casino : RustLegacyPlugin	 
	{
			int counter  = 0;
			string chatName = "КАЗИНО";
			private System.Random getrandom;

			
						void Init() { UnityEngine.Debug.Log("[CASINO PLUGIN] RustExtended : Plugin casino is loaded");	 }
						void Loaded() {	getrandom = new System.Random(); }
			


		[ChatCommand("casino")]
		void casino(NetUser netuser, string command, string[] args)		
		{
                    rust.SendChatMessage(netuser, chatName, ("добро пожаловать в казино"));
					rust.SendChatMessage(netuser, chatName, ("используй /casall"));
	
		}
		[ChatCommand("casall")]
		void casall(NetUser netuser, string command, string[] args)
		{
			
		
		ulong balance = Economy.GetBalance(netuser.userID);
		
		if(balance >= 5000) {
			counter++;
				int rnd=getrandom.Next(1, 4);
				if(rnd == 1) 
				{ 
				Economy.BalanceSub(netuser.userID, 1000);
				rust.SendChatMessage(netuser, chatName, (counter + " Нам очень жаль! Вы все проиграли ( [COLOR#388FFF]x0 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
				} 
				else if(rnd == 2) {
				Economy.BalanceSub(netuser.userID, 1000);
				rust.SendChatMessage(netuser, chatName, (counter + " Нам очень жаль! Вы проиграли ( [COLOR#388FFF]1000 [COLOR#FFFAFA]). Вам баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								  }
				else if(rnd == 3) {Economy.BalanceAdd(netuser.userID, 5000); rust.SendChatMessage(netuser, chatName, (counter + " Поздравляем вы выиграли ( [COLOR#388FFF]5000 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));}
		}
		else {rust.SendChatMessage(netuser, chatName, ("Баланс меньше 5000, ты не можешь играть в казинo!"));}
		}
		

	
					
					
					
		
		
		
		
		
		
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