using System;
using System.Collections.Generic;
using Oxide.Core.Libraries;
using UnityEngine;
using RustExtended;
#pragma warning disable 0618 
// Final Release 06.10.2022
namespace Oxide.Plugins
{
    [Info("Casino", "kustanovich", "2.0.1")]
    class Casino : RustLegacyPlugin	 
	{
	void Init()   { UnityEngine.Debug.Log("[CASINO PLUGIN(Final Release)] RustExtended : Plugin casino is loaded"); }
	void Loaded() {	getrandom = new System.Random(); }
				
	string chatName = "КАЗИНО";
	int[] wins = {1000, 2500, 5000, 10000, 20000, 50000, 100000};
	int[] chance = {50, 50, 50, 40, 40, 30, 30};
	private System.Random getrandom;
					
	[ChatCommand("cas")] 
	void bet(NetUser netuser, string command, string[] args){
				
				ulong money = Economy.GetBalance(netuser.userID);
				int rnd1 = getrandom.Next(1, 3);
				int rnd2 = getrandom.Next(1, 4);
				int rnd3 = getrandom.Next(1, 5);
				
				string cmd = args[0];
				if(cmd == "1000") 
				{
					if(money < 1000) { rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в ставочное казино, баланс менньше : [COLOR#388FFF]" + cmd)); }
					else { 
							if(rnd1 == 1) 
							{
								Economy.BalanceAdd(netuser.userID, 1000);
								rust.SendChatMessage(netuser, chatName, ("Поздравляем! Вы выиграли от своей ставки ( [COLOR#388FFF]x2 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
							}
							else if (rnd1 == 2) 
							{					
								Economy.BalanceSub(netuser.userID, 1000);
								rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								}
							else if (rnd1 == 3) {}
					
				
							}
								}
				else if(cmd == "5000") {
					
					if(money < 5000) { rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в ставочное казино, баланс меньше : [COLOR#388FFF]" + cmd)); }
					else { 
							if(rnd1 == 1) 
							{
								Economy.BalanceAdd(netuser.userID, 5000);
								rust.SendChatMessage(netuser, chatName, ("Поздравляем! Вы выиграли от своей ставки ( [COLOR#388FFF]x2 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
							}
							else if (rnd1 == 2) 
							{					
								Economy.BalanceSub(netuser.userID, 5000);
								rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								}
							else if (rnd1 == 3) {}
					
				}
	}
	
					else if(cmd == "2500") {
					
					if(money < 2500) { rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в ставочное казино, баланс меньше : [COLOR#388FFF]" + cmd)); }
					else { 
							if(rnd1 == 1) 
							{
								Economy.BalanceAdd(netuser.userID, 2500);
								rust.SendChatMessage(netuser, chatName, ("Поздравляем! Вы выиграли от своей ставки ( [COLOR#388FFF]x2 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
							}
							else if (rnd1 == 2) 
							{					
								Economy.BalanceSub(netuser.userID, 2500);
								rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								}
							else if (rnd1 == 3) {}
					
				}
	}
	
						else if(cmd == "10000") {
					
					if(money < 10000) { rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в ставочное казино, баланс меньше : [COLOR#388FFF]" + cmd)); }
					else { 
							if(rnd2 == 1) 
							{
								Economy.BalanceAdd(netuser.userID, 10000);
								rust.SendChatMessage(netuser, chatName, ("Поздравляем! Вы выиграли от своей ставки ( [COLOR#388FFF]x2 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
							}
							else if (rnd2 == 2 || rnd2 == 3) 
							{					
								Economy.BalanceSub(netuser.userID, 10000);
								rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								}
									
							else if (rnd2 == 4) {}
					
				}
	}
	
				else if(cmd == "20000") {
					
					if(money < 20000) { rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в ставочное казино, баланс меньше : [COLOR#388FFF]" + cmd)); }
					else { 
							if(rnd2 == 1) 
							{
								Economy.BalanceAdd(netuser.userID, 20000);
								rust.SendChatMessage(netuser, chatName, ("Поздравляем! Вы выиграли от своей ставки ( [COLOR#388FFF]x2 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
							}
							else if (rnd2 == 2 || rnd2 == 3) 
							{					
								Economy.BalanceSub(netuser.userID, 20000);
								rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								}
									
							else if (rnd2 == 4) {}
					
				}
	}
	
	
				else if(cmd == "50000") {
					
					if(money < 50000) { rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в ставочное казино, баланс меньше : [COLOR#388FFF]" + cmd)); }
					else { 
							if(rnd3 == 1) 
							{
								Economy.BalanceAdd(netuser.userID, 50000);
								rust.SendChatMessage(netuser, chatName, ("Поздравляем! Вы выиграли от своей ставки ( [COLOR#388FFF]x2 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
							}
							else if (rnd3 == 2 || rnd2 == 3 || rnd3 == 4) 
							{					
								Economy.BalanceSub(netuser.userID, 50000);
								rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								}
									
							else if (rnd3 == 5) {rust.SendChatMessage(netuser, chatName, ("Остались при себе. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));}
					
				}
	}
	
					else if(cmd == "100000") {
					
					if(money < 100000) { rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в ставочное казино, баланс меньше : [COLOR#388FFF]" + cmd)); }
					else { 
							if(rnd3 == 1) 
							{
								Economy.BalanceAdd(netuser.userID, 100000);
								rust.SendChatMessage(netuser, chatName, ("Поздравляем! Вы выиграли от своей ставки ( [COLOR#388FFF]x2 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
							}
							else if (rnd3 == 2 || rnd2 == 3 || rnd3 == 4) 
							{					
								Economy.BalanceSub(netuser.userID, 100000);
								rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								}
									
							else if (rnd3 == 5) {rust.SendChatMessage(netuser, chatName, ("Остались при себе. Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));}
					
				}
	}

	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	}
	


					
				
			
			


		[ChatCommand("casino")]
		void casino(NetUser netuser, string command, string[] args)		
		{
					int counter = 0;
                    rust.SendChatMessage(netuser, chatName, ("Добро пожаловать в казино, тут ты можешь все проиграть, а можешь и выиграть."));
					rust.SendChatMessage(netuser, chatName, ("Если ты хочешь играть на ставки, используй команду [COLOR#388FFF]/cas [COLOR#FFFAFA]и следующие значения : "));
					foreach(var item in wins) 
					{
					rust.SendChatMessage(netuser, chatName, ("Доступные ставки : [COLOR#388FFF]" + item + "      [COLOR#FFFAFA]Шанс на x2  [COLOR#388FFF]: " + chance[counter] + "%"));
						counter++;
					}
					rust.SendChatMessage(netuser, chatName, ("Полагайся на свою удачу. Для входа в казино [COLOR#388FFF]All-in[COLOR#FFFAFA], используй команду [COLOR#388FFF]/casall"));
					rust.SendChatMessage(netuser, chatName, ("Минимальный баланс для входа в [COLOR#388FFF]All-in[COLOR#FFFAFA]: [COLOR#388FFF]5000"));
	
		}
		
		[ChatCommand("casall")]
		void casall(NetUser netuser, string command, string[] args)
		{
					ulong balance = Economy.GetBalance(netuser.userID);

		
		if(balance >= 5000) {

		int rnd=getrandom.Next(1, 9);
				if(rnd == 1) {  Economy.BalanceSub(netuser.userID, balance); 
				} 
				else if(rnd == 2) {
				Economy.BalanceSub(netuser.userID, balance);
				rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы все проиграли ( [COLOR#388FFF]x0 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								  }
				else if(rnd == 3) {
					Economy.BalanceAdd(netuser.userID, 1000); 
					rust.SendChatMessage(netuser, chatName, ("Поздравляем вы выиграли ( [COLOR#388FFF]" + wins[1] + " [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
					}
				else if(rnd == 4) {
					Economy.BalanceAdd(netuser.userID, 5000);
					rust.SendChatMessage(netuser, chatName, ("Поздравляем вы выиграли ( [COLOR#388FFF]5000 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
								}
								else if (rnd == 5) {
									Economy.BalanceSub(netuser.userID, 5000); 
										rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли ( [COLOR#388FFF] 5000 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
									}
									else if (rnd == 6) {
									Economy.BalanceSub(netuser.userID, 10000); 
										rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы проиграли ( [COLOR#388FFF] 10000 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
									
									}
									else if (rnd == 7) {
										
									Economy.BalanceAdd(netuser.userID, 10000); 
										rust.SendChatMessage(netuser, chatName, ("Поздравляем вы выиграли ( [COLOR#388FFF] 5000 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
									}
									else if (rnd == 8) 
										{
										Economy.BalanceAdd(netuser.userID, 50000) ;
	       rust.SendChatMessage(netuser, chatName, ("Поздравляем вы выиграли ( [COLOR#388FFF] 50000 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
									}
									else if (rnd == 9) 
										{ 
				Economy.BalanceSub(netuser.userID, balance);
				rust.SendChatMessage(netuser, chatName, ("Нам очень жаль! Вы все проиграли ( [COLOR#388FFF]x0 [COLOR#FFFAFA]). Ваш баланс : [COLOR#388FFF]" + Economy.GetBalance(netuser.userID)));
				} 
										
		}
									
									
		else {rust.SendChatMessage(netuser, chatName, ("Вы не можете играть в казино, потому что баланс меньше : [COLOR#388FFF]5000"));}
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