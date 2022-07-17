using System;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Libraries;
using RustExtended;

namespace Oxide.Plugins
{
    [Info("Showdamage", "kustanovich", 1.0,0)]
    class Showdamage : RustLegacyPlugin
    {
		void Loaded()
        {
			if (!permission.PermissionExists("DamageStatus")) permission.RegisterPermission("DamageStatus", this);   

        }
		bool hasDamage(NetUser netuser, string permissionname)
        {
            if (permission.UserHasPermission(netuser.playerClient.userID.ToString(), "DamageStatus")) return true;
            return permission.UserHasPermission(netuser.playerClient.userID.ToString(), permissionname);
        }
		void ModifyDamage(TakeDamage takedamage, DamageEvent damage)
        {
			if ((damage.attacker.client!=null)&&(damage.attacker.client!=damage.victim.client)&&(damage.victim.client!=null)&&(damage.damageTypes != DamageTypeFlags.damage_explosion)&&(damage.attacker.client.rootControllable.idMain.GetComponent<Inventory>().activeItem.datablock.name.Contains("Shotgun")))return;
			if ((damage.attacker.client!=null)&&(damage.attacker.client!=damage.victim.client)&&(damage.victim.client!=null)&&(damage.damageTypes != DamageTypeFlags.damage_explosion))
			{
			if (hasDamage(damage.attacker.client.netUser, "DamageStatus"))
				{
				rust.InventoryNotice(damage.attacker.client.netUser, + Convert.ToInt32(damage.amount) + " HP" );
				}
				else {return;}
			}
			return;
		}
		[ChatCommand("damage")]
			void Damagetoggle(NetUser netuser, string command, string[] args)
			{
				if (!hasDamage(netuser, "DamageStatus"))
				{
					var tipeone = " DamageStatus";
					var userid = netuser.userID;
					rust.RunServerCommand("oxide.grant user " + userid + tipeone);
					Broadcast.Message(netuser, "[COLOR#FFFAFA]ВЫ [COLOR#388FFF]ВКЛЮЧИЛИ [COLOR#FFFAFA]оповещение об уроне");
				}
				else if (hasDamage(netuser, "DamageStatus"))					
				{
					var tipeone = " DamageStatus";
					var userid = netuser.userID;
					rust.RunServerCommand("oxide.revoke user " + userid + tipeone);
					Broadcast.Message(netuser, "[COLOR#FFFAFA]ВЫ [COLOR#388FFF]ОТКЛЮЧИЛИ [COLOR#FFFAFA]оповещение об уроне");
				}
			}
    }
}