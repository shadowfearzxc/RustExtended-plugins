using System;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Libraries;
using RustExtended;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Blockitem", "edited by kustanovich", 1.0)]
    class Blockitem : RustLegacyPlugin
    {
        IInventoryItem item;
        public DateTime DeniedInvtims = new DateTime(2020, 05, 05, 17, 00, 00); //(Год,Месяц,День,Час,Минуты,Секунды)	
        public Dictionary<string, DateTime> itemtime = new Dictionary<string, DateTime>();
        DateTime outputDateTimeValue;
        public string[] rezka(string text, string inx)
        {
            string[] itemnom = new string[] { inx };
            string[] result = text.Split(itemnom, StringSplitOptions.RemoveEmptyEntries);
            return result;
        }
        void Init()
        {
                   DateTime CurTime = DateTime.Now;
                        string timestamp = CurTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Puts("time1 xxxxx  " + timestamp);
            DateTime convertedDate = DateTime.Parse(timestamp);
            DateTime CurTime2 = convertedDate.AddSeconds(3600);
            string timestampw = CurTime2.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Puts("time2 xxxxx  " + timestampw);
           if (!Config.Exists())
            {
            LoadDefaultConfig();
            } 
            else
           { 
                Config.Load();
                string blockitem = (string)Config["blockitem"];
                string[] xxxx = rezka(blockitem, ",");
                itemtime.Clear();
                foreach (var ccc in xxxx)
                {
                    string[] nametime = rezka(ccc, ":");
                    if (nametime.Length < 2)
                    {
                       LoadDefaultConfig();
                       return;
                    }
                    string[] timex = rezka(nametime[1], "-");
                   if (timex.Length <= 4)
                    {
                       LoadDefaultConfig();
                       return;
                    }
                    DeniedInvtims = new DateTime(Convert.ToInt32(timex[0]), Convert.ToInt32(timex[1]), Convert.ToInt32(timex[2]), Convert.ToInt32(timex[3]), Convert.ToInt32(timex[4]), Convert.ToInt32(timex[5])); //(Год,Месяц,День,Час,Минуты,Секунды)		
                    itemtime.Add(nametime[0], DeniedInvtims);
                    Puts(nametime[0] + " : " + DeniedInvtims.ToString());
                } 
		   }
        } 
      void OnItemDeployed(DeployableObject deployable, IDeployableItem item)
        {
                if (item.datablock != null)
                {
                    if (item.datablock.name != "")
                    {
                        string name = item.datablock.name;
                        DateTime CurTime = DateTime.Now;
                        if (itemtime.ContainsKey(name) && CurTime <= itemtime[name])
                        {
                            rust.SendChatMessage(item.character.netUser, "REBORN",
                                string.Format("Вы [COLOR#388FFF]нарушили [COLOR#FFFAFA]правило использования предметов при блокировке, [COLOR#388FFF]" + name + " [COLOR#FFFAFA]удален!"));
                            NetCull.Destroy(deployable.gameObject);

                        }
                    }              
            }   
        }
         protected override void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file");
            Config.Clear();
            string items = "M4:2020-05-05-17-00-00,Revolver:2020-05-05-17-00-00,Shotgun:2020-05-05-17-00-00,Explosive Charge:2020-05-05-17-00-00,F1 Grenade:2020-05-05-17-00-00,P250:2020-05-05-17-00-00,MP5A4:2020-05-05-17-00-00,Rock:2020-05-05-17-00-00";
            Config["blockitem"] = items;
            SaveConfig();
            Config.Load();
            string blockitem = (string)Config["blockitem"];
            string[] xxxx = rezka(blockitem, ",");
            itemtime.Clear();
            foreach (var ccc in xxxx)
            {
                string[] nametime = rezka(ccc, ":");
                if (nametime.Length < 2)
                {
                    LoadDefaultConfig();
                    return;
                }
                string[] timex = rezka(nametime[1], "-");
                if (timex.Length <= 4)
                {
                    LoadDefaultConfig();
                    return;
                }
                DeniedInvtims = new DateTime(Convert.ToInt32(timex[0]), Convert.ToInt32(timex[1]), Convert.ToInt32(timex[2]), Convert.ToInt32(timex[3]), Convert.ToInt32(timex[4]), Convert.ToInt32(timex[5])); //(Год,Месяц,День,Час,Минуты,Секунды)		
                itemtime.Add(nametime[0], DeniedInvtims);
                Puts(nametime[0] + " : " + DeniedInvtims.ToString());
            }
        }  
        bool hasAccess(NetUser player)
        {
            if (!player.CanAdmin())
            {
                return false;
            }
            return true;
        }
        void OnGetClientMove(HumanController controller, Vector3 newPos)
        {
            DateTime CurTime = DateTime.Now;
                var netuser = controller.netUser;
                var inv = netuser.playerClient.controllable.GetComponent<PlayerInventory>();
                if (inv != null && inv.activeItem != null)
                {
                    if (!hasAccess(netuser))
                    {
                        var name = inv.activeItem.datablock.name.ToString();               
                        if (itemtime.ContainsKey(name) && CurTime <= itemtime[name])
                        {
                             System.TimeSpan fulltime = itemtime[name].Subtract(CurTime);
                                rust.SendChatMessage(netuser, "REBORN",
                                    string.Format("Предмет[COLOR#388FFF] " + name + " [COLOR#FFFAFA]запрещено использовать еще [COLOR#388FFF]" +
                                                  Convert.ToInt32(fulltime.TotalMinutes) + " [COLOR#FFFAFA]минут"));
                                inv.DeactivateItem();
                            
                        }
                    }
                }          
        }
    }
}