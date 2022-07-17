// Reference: Facepunch.ID
// Reference: Facepunch.MeshBatch
// Reference: Google.ProtocolBuffers
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Data;
using System.Globalization;
using UnityEngine;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Core.Configuration;
using RustExtended;
#pragma warning disable 0618 // отключение предупреждений об устаревших методах
namespace Oxide.Plugins
{
    [Info("Craft", "edited by kustanovich", "1.0.0")]
    class Craft : RustLegacyPlugin
    {
        public static Craft singleton;
        #region rezka
        public static string[] rezka(string text, string inx)
        {
            return singleton.rezkax(text, inx);
        }
        public string[] rezkax(string text, string inx)
        {
            string[] itemnom = new string[] { inx };
            string[] result = text.Split(itemnom, StringSplitOptions.RemoveEmptyEntries);
            return result;
        }
        #endregion
		bool TryGetPlayerView(NetUser player, out UnityEngine.Quaternion viewAngle)
        {
            viewAngle = new UnityEngine.Quaternion(0f, 0f, 0f, 0f);
            if (player.playerClient.rootControllable == null) return false;
            viewAngle = player.playerClient.rootControllable.GetComponent<Character>().eyesRotation;
            return true;
        }
        public bool LookAtPosition(PlayerClient player, out Vector3 position, float maxDistance = 100f)
        {
            RaycastHit hit;
            position = new Vector3(0f, 0f, 0f);
            Character idMain = player.controllable.idMain;
            if ((idMain != null) && Physics.Raycast(idMain.eyesRay, out hit, maxDistance, -1))
            {
                Vector3 vector;
                TransformHelpers.GetGroundInfo(hit.point, 100f, out position, out vector);
                return true;
            }
            return false;
        }
        public int Percent(double number,int percent)
        {
            double cv = ((double)number * percent) / 100;
            return Convert.ToInt32(Math.Ceiling(cv));
        }
        #region craftb : MonoBehaviour
        public class craftb : MonoBehaviour
        {
            public PlayerClient playerClient;
            public craftb xxxc;
            public float speed;
            public DeployableObject objct;
            public LootableObject lootobject;
            public Inventory inv;
            void Awake()
            {
                xxxc = this;
                objct = GetComponent<DeployableObject>();
                playerClient = Helper.GetPlayerClient(objct.ownerID);
                objct.GrabCarrier();
                lootobject = objct.GetComponent<LootableObject>();
                inv = lootobject._inventory;
            }
            public void checkdestroy()
            {
                if (playerClient == null) this.OnDestroy();
                if (Vector3.Distance(playerClient.lastKnownPosition, objct.collider.bounds.center) > 30f)
                {
                    if (Time.time - this.speed <= 2.5f ? false : true)
                    {
                        this.speed = Time.time;
                        mesage(playerClient.netUser, "Переработчик был [COLOR#338FFF]УНИЧТОЖЕН");
                        this.OnDestroy();
                    }
                }
                if (Vector3.Distance(playerClient.lastKnownPosition, objct.collider.bounds.center) > 10f)
                {
                    if (Time.time - this.speed <= 2.5f ? false : true)
                    {
                        this.speed = Time.time;
                        mesage(playerClient.netUser,
                            "Вы находитесь от переработчика в [COLOR#388FFF]" + Vector3.Distance(playerClient.lastKnownPosition, objct.collider.bounds.center).ToString() + " [COLOR#FFFAFA]метрах. Переработчик будет [COLOR#388FFF]УНИЧТОЖЕН");
                    }
                }
            }
            void FixedUpdate()
            {
                checkdestroy();
            }
            public void OnDestroy()
            {
                NetCull.Destroy(objct.gameObject);
                GameObject.Destroy(xxxc);
            }
        }
        #endregion
        bool hasAccess(NetUser netuser)
        {
            if (netuser.CanAdmin()) return true;
            return false;
        }
        public static void mesage(NetUser netus, string text)
        {
            singleton.sms(netus, text);
        }
        public void sms(NetUser netus, string text)
        {
            SendReply(netus, text);
        }
        Dictionary<string, DeployableObject> xx = new Dictionary<string, DeployableObject>();
        public DeployableObject NewObject(Vector3 pos, Quaternion ror, NetUser user)
        {

            var lastObjectx = NetCull.InstantiateStatic(";deploy_wood_storage_large", pos, ror);
            var lastObject = lastObjectx.GetComponent<DeployableObject>();
            lastObject.SetupCreator(user.playerClient.rootControllable.character.controllable);
            return lastObject;
        }
        public bool FBI(ItemDataBlock item, out BlueprintDataBlock blueprint)
        {
            ItemDataBlock[] all = DatablockDictionary.All;
            for (int i = 0; i < all.Length; i++)
            {
                BlueprintDataBlock blueprintDataBlock = all[i] as BlueprintDataBlock;
                if (blueprintDataBlock != null && blueprintDataBlock.resultItem == item)
                {
                    blueprint = blueprintDataBlock;
                    return true;
                }
            }
            blueprint = null;
            return false;
        }
        public int nedslot(BlueprintDataBlock blueprint, int x, int proc)
        {
            double item = 0;
            var xc = blueprint.ingredients;
            foreach (var asd in xc)
            {
                double sss = Convert.ToDouble(Percent(Convert.ToDouble(asd.amount) * Convert.ToDouble(x), proc));
                item += sss / Convert.ToDouble(asd.Ingredient._maxUses);
            }
            return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(item)));
        }
        public string blist(BlueprintDataBlock blueprint, int x, int proc)
        {
            string item = "";
            var xc = blueprint.ingredients;
            foreach (var asd in xc)
            {
                item += String.Format("[{0}:{1}],", asd.Ingredient.name, Percent(asd.amount * x, proc));
            }
            return item;
        }
        public void additem(BlueprintDataBlock blueprint, int x, int proc, Inventory inv)
        {
            string item = "";
            var xc = blueprint.ingredients;
            foreach (var asd in xc)
            {
                inv.AddItemAmount(asd.Ingredient, Percent(asd.amount * x, proc));
            }
        }
		public bool find(Vector3 pos, out StructureComponent fbuildingblock)
        {
            fbuildingblock = null;
            //  DeployableObject fdeployable;
            foreach (var hit in Facepunch.MeshBatch.MeshBatchPhysics.OverlapSphere(pos + new Vector3(0f, 0.5f, 0f), 1f))
            {
                if (hit.name.Contains("Foundation") || hit.name.Contains("Ceiling") || hit.name.Contains("Stairs") || hit.name.Contains("Ramp"))
                {
                    if (hit.GetComponentInParent<StructureComponent>() != null)
                    {
                        fbuildingblock = hit.GetComponentInParent<StructureComponent>();
                        return true;
                    }
                }
            }
            return false;
        }
        public List<string> blocklist()
        {
            var xx = new List<string>();
            string[] x = rezka(conf("blockuncraft"), ",");
            if (x.Length > 0)
            {
                foreach (var xxv in x)
                {
                    xx.Add(xxv);
                }
            }
            return xx;
        }
        public string conf(string name)
        {
            Config.Load();
            string index = (string)Config[name];
            if (!string.IsNullOrEmpty(index))
            {
                return index;
            }
            if (name == "proc")
            {
                Config[name] = "50";
            }
            else
            {
                Config[name] = "null,";
            }          
            SaveConfig();
            return conf(name);
        }
        [ChatCommand("u")]
        void asd(NetUser netuser, string command, string[] args)
        {
            string id = netuser.userID.ToString();
            Inventory inventory = netuser.playerClient.controllable.GetComponent<Inventory>();
            DeployableObject crafter;
            if (!xx.ContainsKey(id) && !xx.TryGetValue(id, out crafter))
            {
                UnityEngine.Quaternion currentRot;
                if (!TryGetPlayerView(netuser, out currentRot))
                {
                    SendReply(netuser, "Couldn't find your eyes");
                    return;
                }
                Vector3 vector;
                if (!LookAtPosition(netuser.playerClient, out vector, 100f))
                {
                    Broadcast.Message(netuser, "Вы не можете поставить переработчик. Причина:[COLOR#388FFF] наведите взгляд на свой фундамент", null, 0f);
                    return;
                }
                StructureComponent fbuildingblock;
                if (!find(vector, out fbuildingblock))
                {
                    Broadcast.Message(netuser, "Вы не можете поставить переработчик. Причина:[COLOR#388FFF] наведите взгляд на свой фундамент", null, 0f);
                    return;
                }
                if (fbuildingblock._master.ownerID != netuser.userID)
                {

                    Broadcast.Message(netuser, "Вы не можете поставить переработчик. Причина: [COLOR#388FFF]Фундамент/потолок [COLOR#FFFAFA]не принадлежит вам!", null, 0f);
                    return;
                }
                ItemDataBlock ItemOut1 = DatablockDictionary.GetByName("Wood");
                int SoldAmount1 = RustExtended.Helper.InventoryItemCount(inventory, ItemOut1);
                if (SoldAmount1 >= 30)
                {
                    RustExtended.Helper.InventoryItemRemove(inventory, ItemOut1, Convert.ToInt32(30));
                }
                else
                {
                    Broadcast.Message(netuser, "Вы не можете поставить переработчик. Причина: необходимо [COLOR#388FFF]30 [COLOR#FFFAFA]дерева", null, 0f);
                    return;
                }
                Vector3 OriginRotation = new Vector3(0f, currentRot.y, 0f);
                UnityEngine.Quaternion OriginRot = UnityEngine.Quaternion.EulerRotation(OriginRotation);
                crafter = NewObject(vector, OriginRot, netuser);
                crafter.gameObject.AddComponent<craftb>();
                xx.Add(id, crafter);
                asd(netuser, command, args);
            }
            else
            {
                int proc = Convert.ToInt32(conf("proc"));
                xx.TryGetValue(id, out crafter);
                if (crafter == null)
                {
                    xx.Remove(id);
                    asd(netuser, command, args);
                    return;
                }
                var hh = crafter.GetComponent<craftb>();
                if (args.Length > 0)
                {
                    string comanda = args[0];
                    var inv = hh.inv;
                    if (comanda == "all")
                    {
                        string x = inventorylistitem(inv);
                        string[] item = rezka(x, ";");
//sms(netuser, String.Format("[COLOR#FFFAFA]При переработке предметов вы получаете [COLOR#388FFF]{0}%", proc));
                        foreach (var itemcol in item)
                        {
                            if (!itemcol.Contains("пусто"))
                            {
                                string[] itemx = rezka(itemcol, ":");
                                int frees = freeslot(inv);
                                sms(netuser, String.Format("Свободных слотов в переработчике [COLOR#388FFF]{0}", frees));
                                ItemDataBlock unitem = DatablockDictionary.GetByName(itemx[0]);
                                BlueprintDataBlock pb;
                                FBI(unitem, out pb);
                                if (pb != null)
                                {
                                    if (!blocklist().Contains(itemx[0]))
                                    {
                                        int nedsl = nedslot(pb, Convert.ToInt32(itemx[1]), proc);
                                        if (nedsl < frees)
                                        {
                                            sms(netuser,
                                                String.Format("Начался процесс разбора [COLOR#388FFF]{0}[COLOR#FFFAFA] / Количество : [COLOR#388FFF]{1}[COLOR#FFFAFA] / Получено [COLOR#388FFF]{2}", itemx[0], itemx[1],
                                                    blist(pb, Convert.ToInt32(itemx[1]), proc)));
                                            InventoryItemRemove(inv, unitem, Convert.ToInt32(itemx[1]));
                                            additem(pb, Convert.ToInt32(itemx[1]), proc, inv);
                                        }
                                        else
                                        {
                                            sms(netuser,
                                                String.Format("В переработчике заполнены все места, необходимо освободить[COLOR#388FFF]{0} [COLOR#FFFAFA]слотов",
                                                    nedslot(pb, Convert.ToInt32(itemx[1]), proc)));
                                        }
                                    }
                                    else
                                    {
                                        sms(netuser, String.Format("В переработчике находится [COLOR#388FFF]запрещенный [COLOR#FFFAFA]предмет для переработки [COLOR#388FFF]{0} [COLOR#FFFAFA]: [COLOR#388FFF]{1} ", itemx[0], itemx[1]));
                                    }
                                }
                                else
                                {
                                    sms(netuser, String.Format("В переработчике находится предмет, который нельзя разобрать [COLOR#388FFF]{0} [COLOR#FFFAFA]: [COLOR#388FFF]{1} ", itemx[0], itemx[1]));
                                }
                            }
                            else
                            {
                                // sms(netuser, "Пустой слот");
                            }
                        }
                    }

                }
                else
                {
                    Broadcast.Message(netuser, "При переработке предметов вы получаете [COLOR#388FFF]50%");
                    Broadcast.Message(netuser, "Начинает переработку всех [COLOR#388FFF]предметов [COLOR#FFFAFA]находящихся в переработчике. Использование : [COLOR#388FFF]/u all");
                    Broadcast.Message(netuser, "Переработчик пропадет, если отойти на расстояние в [COLOR#388FFF]30 [COLOR#FFFAFA]метров, вещи также в нем [COLOR#388FFF]пропадут[COLOR#FFFAFA]!");
                    Broadcast.Message(netuser, "Переработчик также можно сломать. Использование : [COLOR#388FFF]/destroy");
                }
            }
        }
        void Init()
        {
            if (!Config.Exists())
            {
                LoadDefaultConfig();
            }

        }
        protected override void LoadDefaultConfig()
        {
            Config["blockuncraft"] = "null,";
            SaveConfig();
        }
        void Loaded()
        {
            singleton = this;
        }
        void OnServerInitialized()
        {
            singleton = this;

        }
        public int InventoryItemRemove(Inventory inventory, ItemDataBlock datablock, int quantity)
        {
            int i = 0;
            while (i < quantity)
            {
                IInventoryItem inventoryItem = inventory.FindItem(datablock);
                if (inventoryItem == null)
                {
                    break;
                }
                if (!inventoryItem.datablock.IsSplittable())
                {
                    i++;
                    inventory.RemoveItem(inventoryItem);
                }
                else
                {
                    int num = quantity - i;
                    if (inventoryItem.uses > num)
                    {
                        i += num;
                        inventoryItem.SetUses(inventoryItem.uses - num);
                    }
                    else
                    {
                        i += inventoryItem.uses;
                        inventory.RemoveItem(inventoryItem);
                    }
                }
            }
            return i;
        }
        public static string inventorylistitem(Inventory inventory)
        {
            return singleton.inventorylistitemx(inventory);
        }
        public string inventorylistitemx(Inventory inventory)
        {
            IInventoryItem item;
            string num = "";
            if (inventory != null)
            {
                for (int i = 0; i < inventory.slotCount; i++)
                {
                    inventory.GetItem(i, out item);
                    if (item != null)
                    {
                        //Debug.Log(item.datablock.name.ToString());

                        if (item.datablock.IsSplittable())
                        {
                            num += String.Format("{0}:{1};", item.datablock.name, item.uses);
                        }
                        else
                        {
                            num += String.Format("{0}:{1};", item.datablock.name, 1);
                        }
                    }
                    else
                    {
                        num += String.Format("пусто:пусто;");
                    }
                }
            }
            return num;
        }
        public int freeslot(Inventory inventory)
        {
            IInventoryItem item;
            int num = 0;
            if (inventory != null)
            {
                for (int i = 0; i < inventory.slotCount; i++)
                {
                    inventory.GetItem(i, out item);
                    if (item != null)
                    {
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            return num;
        }
    }
}
