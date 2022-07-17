using System;
using System.Collections.Generic;
using RustExtended;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Trader", "edited by kustanovich", 1)]
    [Description("Trade for players")]
    class Trader : RustLegacyPlugin
    {
        string chatName = "REBORN";
        string mainColor = "[COLOR #FFFAFA]";
        string secondColor = "[COLOR# 388FFF]";
        string successColor = "[COLOR #00FF00]";
        ulong timeOutAccept = 15;
        ulong timeOutTrade = 100;
        static TradeData tradeData;

        class UserInfo
        {
            public ulong steamID;           
            public DealInfo deal;
            public static UserInfo getBySteamID(ulong steamID)
            {
                foreach (UserInfo info in tradeData.UserInfo)
                {
                    if (info != null && info.steamID == steamID)
                    {
                        return info;
                    }
                }
                return null;
            }
            public UserInfo(ulong userID)
            {
                steamID = userID;
                deal = null;
            }
        }
        class DealInfo
        {
            public int dealID;
            public ulong startReqTime;
            public ulong startDealTime;
            public bool accepted;
            public ulong leaderId;
            public ulong traderId;
            public IInventoryItem leaderItem;
            public IInventoryItem traderItem;
            public int leaderCount;
            public int traderCount;
            public bool leaderConfirmed;
            public bool traderConfirmed;
            public DealInfo(ulong fromUserId, ulong toUserID)
            {
                dealID = UnityEngine.Random.Range(1000, 1000000000);
                startReqTime = NetCull.timeInMillis;
                leaderId = fromUserId;
                traderId = toUserID;
            }
            public bool Confirmed()
            {
                if (leaderConfirmed && traderConfirmed && leaderItem != null && traderItem != null)
                    return true;

                return false;
            }
            public void Remove()
            {
                if (UserInfo.getBySteamID(leaderId) != null)
                    UserInfo.getBySteamID(leaderId).deal = null;
                if (UserInfo.getBySteamID(traderId) != null)
                    UserInfo.getBySteamID(traderId).deal = null;
                foreach (DealInfo deal in Trader.tradeData.DealInfo)               
                if (deal.dealID == dealID)
                {
                    tradeData.DealInfo.Remove(deal);
                    Interface.GetMod().DataFileSystem.WriteObject("TradeData", tradeData);
                    break;
                }               
            }
            public static DealInfo getByLeaderID(ulong steamID)
            {
                foreach (DealInfo info in tradeData.DealInfo)
                {
                    if (info.leaderId == steamID)
                    {
                        return info;
                    }
                }
                return null;
            }
            public static DealInfo getByTraderID(ulong steamID)
            {
                foreach (DealInfo info in tradeData.DealInfo)
                {
                    if (info.traderId == steamID)
                    {
                        return info;
                    }
                }
                return null;
            }
        }
        class TradeData
        {
            public List<UserInfo> UserInfo = new List<UserInfo> { };
            public List<DealInfo> DealInfo = new List<DealInfo> { };
        }
        void Loaded()
        {
            tradeData = Interface.GetMod().DataFileSystem.ReadObject<TradeData>("TradeData");
        }
        bool UserAtServer(ulong steamId)
        {
            foreach (PlayerClient current in PlayerClient.All)
            if (current != null && current.userID == steamId)           
              return true;
            
            return false;
        }
        IInventoryItem GetItemInInventory(Inventory inv, ItemDataBlock itemdatablock, int count)
        {
            for (int i = 0; i < inv.slotCount; i++)
            {
                IInventoryItem item = null;
                inv.GetItem(i, out item);
                if (item != null)
                {
                    if (item.datablock.Equals(itemdatablock))
                    {
                        if (item.datablock is WeaponDataBlock)
                        {
                            return item;
                        }
                        else if (item.uses == count)
                        {
                            return item;
                        }
                    }
                }
            }
            return null;
        }
        void AddItem(Inventory inv, int slot, ItemDataBlock dataBlock, int uses)
        {
            Inventory.Slot.Kind belt = Inventory.Slot.Kind.Default;
            if ((slot > 0x1d) && (slot < 0x24))
            {
                belt = Inventory.Slot.Kind.Belt;
            }
            else if ((slot >= 0x24) && (slot < 40))
            {
                belt = Inventory.Slot.Kind.Armor;
            }
            inv.AddItemSomehow(dataBlock, new Inventory.Slot.Kind?(belt), slot, uses);
        }
        int GetFreeSlot(Inventory inv)
        {
            for (int i = 0; i < inv.slotCount; i++)
            {
                if (inv.IsSlotVacant(i))
                {
                    return i;
                }
            }
            return -1;
        }
        ItemDataBlock GetDataBlock(string text)
        {
            foreach (ItemDataBlock item in DatablockDictionary.All)
            if (item != null && item.name.ToLower() == text.ToLower())
                 return item;

            return null;
        }
        void OnFrame()
        {
            if (!Bootstrap.Initialized)
                return;

            foreach (DealInfo deal in tradeData.DealInfo)
            {                              
                if (!deal.accepted && NetCull.timeInMillis - deal.startReqTime > timeOutAccept * 1000)
                {
                    NetUser netLeader = null, netTrader = null;
                    foreach (PlayerClient current in PlayerClient.All)
                    if (current != null)
                    {
                        if (current.userID == deal.leaderId)
                            netLeader = current.netUser;

                        if (current.userID == deal.traderId)
                            netTrader = current.netUser;
                    }

                    if (netLeader != null) rust.SendChatMessage(netLeader, chatName, $"{mainColor}Игрок не успел принять ваш запрос на трейд!");
                    if (netLeader != null) rust.SendChatMessage(netTrader, chatName, $"{mainColor}Время на принятие запроса на трейд вышло!");

                    deal.Remove();
                    return;
                }
                if (deal.accepted && NetCull.timeInMillis - deal.startDealTime > timeOutTrade * 1000)
                {
                    NetUser netLeader = null, netTrader = null;
                    foreach (PlayerClient current in PlayerClient.All)
                        if (current != null)
                        {
                            if (current.userID == deal.leaderId)
                                netLeader = current.netUser;

                            if (current.userID == deal.traderId)
                                netTrader = current.netUser;
                        }

                    if (netLeader != null) rust.SendChatMessage(netLeader, chatName, $"{mainColor}Время для проведения трейда вышло! Сделка отменена.");
                    if (netLeader != null) rust.SendChatMessage(netTrader, chatName, $"{mainColor}Время для проведения трейда вышло! Сделка отменена.");

                    deal.Remove();
                    return;
                }
            } 
        }
        [ChatCommand("trade")]
        void trade(NetUser netUser, string command, string[] args)
        {
            string text = $"Command [{netUser.displayName}:{netUser.userID}] /" + command;
            foreach (string s in args)
                text += " " + s;          
            Helper.LogChat(text, true);
            if (args.Length == 0)
            {
                rust.SendChatMessage(netUser, chatName, $"{mainColor}Список комманд для трейда:");               
                rust.SendChatMessage(netUser, chatName, $"{secondColor}/trade \"ник\"{mainColor} - предложить игроку обмен предметами");
                rust.SendChatMessage(netUser, chatName, $"{secondColor}/trade item \"<название предмета>\" <кол-во>{mainColor} - предложить игроку свой предмет!");
                rust.SendChatMessage(netUser, chatName, $"{secondColor}/trade accept{mainColor} - принять запрос на обмен предметами");
                rust.SendChatMessage(netUser, chatName, $"{secondColor}/trade dismiss{mainColor} - отклонить обмен предметами");
                return;
            }
            ulong UserID = netUser.userID;
            if (args.Length == 1)
            {
                if (args[0] == "item")
                {
                    rust.SendChatMessage(netUser, chatName, $"{mainColor}Использование /trade item \"<название предмета>\" <кол-во>");
                    return;
                }
                if (args[0].ToLower() == "accept")
                {
                    bool userExists = false;
                    foreach (UserInfo info in tradeData.UserInfo)
                    {
                        if (info != null && info.steamID == UserID)
                        {
                            userExists = true;
                            if (info.deal != null && info.deal.leaderId != UserID)
                            {
                                if (info.deal.accepted)
                                {
                                    rust.SendChatMessage(netUser, chatName, $"{secondColor}Запрос уже подтвержден!");
                                    return;
                                }
                                if (netUser.playerClient.controllable == null)
                                {
                                    rust.SendChatMessage(netUser, chatName, $"{secondColor}Вы не можете принять сделку пока вы мертвы!");
                                    return;
                                }
                                info.deal.startDealTime = NetCull.timeInMillis;
                                info.deal.accepted = true;

                                if (!UserAtServer(info.deal.leaderId))
                                {
                                    rust.SendChatMessage(netUser, chatName, $"{secondColor}Игрок, отправивший вам запрос, вышел с сервера. Сделка отменена.");
                                    info.deal.Remove();
                                    return;
                                }
                                rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы {secondColor}подтвердили {mainColor}запрос на обмен предметами");
                                rust.SendChatMessage(netUser, chatName, $"{mainColor}Введите {secondColor}/trade item \"<название предмета>\" <кол-во>{mainColor} чтобы предложить игроку свой предмет!");
                                rust.SendChatMessage(netUser, chatName, $"{mainColor}Введите {secondColor}/trade cancel{mainColor} чтобы отменить обмен!");
                                NetUser netLeader = NetUser.FindByUserID(info.deal.leaderId);
                                if (netLeader != null)
                                {
                                    rust.SendChatMessage(netLeader, chatName, $"{mainColor}Игрок {secondColor}{netUser.displayName} {mainColor}подтвердил ваш запрос на обмен предметами.");
                                    rust.SendChatMessage(netLeader, chatName, $"{mainColor}Введите {secondColor}/trade item \"<название предмета>\" <кол-во>{mainColor} чтобы предложить игроку свой предмет!");
                                    rust.SendChatMessage(netLeader, chatName, $"{mainColor}Введите {secondColor}/trade cancel{mainColor} чтобы отменить обмен!");
                                }
                            }
                            else
                            {
                                rust.SendChatMessage(netUser, chatName, $"{mainColor}У вас {secondColor}нет {mainColor}активных запросов на трейд!");
                            }
                            break;
                        }
                    }
                    if (!userExists)
                    {
                        tradeData.UserInfo.Add(new UserInfo(UserID));
                        Interface.GetMod().DataFileSystem.WriteObject("TradeData", tradeData);

                        rust.SendChatMessage(netUser, chatName, $"{mainColor}У вас {secondColor}нет {mainColor}активных запросов на трейд!");
                    }
                    return;
                }
                if (args[0].ToLower() == "dismiss" || args[0].ToLower() == "cancel")
                {
                    bool userExists = false;
                    foreach (UserInfo info in tradeData.UserInfo)
                    {
                        if (info.steamID == UserID)
                        {
                            userExists = true;

                            if (info.deal != null)
                            {
                                if (info.deal.accepted)
                                 rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы {secondColor}отменили {mainColor}обмен предметами!");
                                else
                                 rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы {secondColor}отказались {mainColor}от обмена предметов!");

                                if (info.deal.leaderId == UserID && UserAtServer(info.deal.traderId))
                                {
                                    NetUser netTrader = NetUser.FindByUserID(info.deal.traderId);
                                    if (netTrader != null)
                                    rust.SendChatMessage(netTrader, chatName, $"{mainColor}Игрок {secondColor}{netUser.displayName}{mainColor} отказался от сделки!");
                                }

                                if (info.deal.traderId == UserID && UserAtServer(info.deal.leaderId))
                                {
                                    NetUser netLeader = NetUser.FindByUserID(info.deal.leaderId);
                                    if (netLeader != null)
                                    rust.SendChatMessage(netLeader, chatName, $"{mainColor}Игрок {secondColor}{netUser.displayName}{mainColor} отказался от сделки!");
                                }

                                info.deal.Remove();
                            }
                            else
                            {
                                rust.SendChatMessage(netUser, chatName, $"{mainColor}У вас {secondColor}нет {mainColor}активных запросов на трейд!");
                            }
                            break;
                        }
                    }
                    if (!userExists)
                    {
                        tradeData.UserInfo.Add(new UserInfo(UserID));
                        Interface.GetMod().DataFileSystem.WriteObject("TradeData", tradeData);

                        rust.SendChatMessage(netUser, chatName, $"{mainColor}У вас {secondColor}нет {mainColor}активных запросов на трейд!");
                    }
                    return;
                }
                if (args[0].ToLower() == "confirm")
                {
                    UserInfo userInfo = null;
                    foreach (UserInfo info in tradeData.UserInfo)
                    {
                        if (info.steamID == UserID)
                        {
                            userInfo = info;
                        }
                    }
                    if (netUser.playerClient.controllable == null)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы {secondColor}не можете {mainColor}подтвердить сделку пока вы {secondColor}мертвы!");
                        return;
                    }
                    if (userInfo == null || userInfo.deal == null)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Сначала предложите игроку обмен предметами: {secondColor}/trade \"ник\"");
                        return;
                    }
                    if (!userInfo.deal.accepted)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Сначало нужно чтобы {secondColor}оба {mainColor}игрока приняли запрос на обмен!");
                        return;
                    }
                    if (userInfo.deal.leaderItem == null || userInfo.deal.traderItem == null)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Сначало нужно чтобы {secondColor}оба игрока предложили свои предметы!");
                        return;
                    }
                    if ((userInfo.deal.traderId == netUser.userID && userInfo.deal.traderConfirmed) || (userInfo.deal.leaderId == netUser.userID && userInfo.deal.leaderConfirmed))
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы уже {secondColor}подтвердили {mainColor}сделку!");
                        return;
                    }
                    NetUser netTarget = null;
                    if (userInfo.deal.traderId == netUser.userID)
                    {
                        netTarget = NetUser.FindByUserID(userInfo.deal.leaderId);

                        if (netTarget == null)
                            Helper.LogChat(">>>>>>>>>>>>>> REBORN >> NetTarget is NULL!", true);

                        userInfo.deal.traderConfirmed = true;
                        if (!userInfo.deal.leaderConfirmed)
                            rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы успешно подтвердили сделку! Ожидайте подтверждения другого игрока!");
                    }
                    if (userInfo.deal.leaderId == netUser.userID)
                    {
                        netTarget = NetUser.FindByUserID(userInfo.deal.traderId);

                        if (netTarget == null)
                            Helper.LogChat(">>>>>>>>>>>>>> REBORN >> NetTarget is NULL!", true);

                        userInfo.deal.leaderConfirmed = true;
                        if (!userInfo.deal.traderConfirmed)
                         rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы успешно {secondColor}подтвердили {mainColor}сделку! Ожидайте подтверждения другого игрока!");
                    }                     
                    if (!userInfo.deal.Confirmed())
                     rust.SendChatMessage(netTarget, chatName, $"{successColor}{netUser.displayName} подтвердил сделку!");

                    FinishTrade(userInfo.deal);
                    return;
                }
                if (args[0].Length > 1)
                {
                    bool found = false;
                    PlayerClient current = Helper.GetPlayerClient(args[0]);
                    if (current != null)
                    {
                        if (current.userID == UserID)
                        {
                            rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы {secondColor}не можете {mainColor}предложить сделку самому себе!");
                            return;
                        }
                        found = true;
                        if (netUser.playerClient.controllable == null)
                        {
                            rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы {secondColor}не можете {mainColor}предлагать сделку пока вы мертвы!");
                            return;
                        }
                        if (current.controllable == null)
                        {
                            rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы {secondColor}не можете {mainColor}предлагать сделку мертвым игрокам!");
                            return;
                        }
                        UserInfo senderInfo = UserInfo.getBySteamID(UserID);
                        UserInfo targetInfo = UserInfo.getBySteamID(current.userID);
                        if (senderInfo == null)
                        {
                            tradeData.UserInfo.Add(new UserInfo(UserID));
                            Interface.GetMod().DataFileSystem.WriteObject("TradeData", tradeData);
                            senderInfo = UserInfo.getBySteamID(UserID);
                        }
                        if (targetInfo == null)
                        {
                            tradeData.UserInfo.Add(new UserInfo(current.userID));
                            Interface.GetMod().DataFileSystem.WriteObject("TradeData", tradeData);
                            targetInfo = UserInfo.getBySteamID(current.userID);
                        }
                        if (senderInfo.deal != null)
                        {
                            rust.SendChatMessage(netUser, chatName, $"{mainColor}Сначала отмените текущую сделку - {secondColor}/trade cancel");
                            return;
                        }
                        if (targetInfo.deal != null)
                        {
                            if (targetInfo.deal.accepted)
                                rust.SendChatMessage(netUser, chatName, $"{secondColor}Игрок сейчас обменивается предметами с другим игроком. Попробуйте отправить запрос позже!");
                            else
                            if (targetInfo.deal.traderId == UserID)
                                rust.SendChatMessage(netUser, chatName, $"{secondColor}Вы уже отправили запрос игроку!");
                            else
                                rust.SendChatMessage(netUser, chatName, $"{secondColor}Игрок еще не ответил на запрос от другого игрока!");
                            return;
                        }
                        DealInfo dealInfo = new DealInfo(UserID, current.userID);
                        senderInfo.deal = dealInfo;
                        targetInfo.deal = dealInfo;                         
                        tradeData.DealInfo.Add(dealInfo);
                        Interface.GetMod().DataFileSystem.WriteObject("TradeData", tradeData);
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы отправили запрос на трейд игроку [COLOR#388FFF]{current.netUser.displayName}[COLOR#FFFAFA]. Ожидайте подтверждения...");
                        rust.SendChatMessage(current.netUser, chatName, $"{mainColor}Игрок {secondColor}{netUser.displayName}{mainColor} предлагает вам обмен предметами. Чтобы принять напишите {secondColor}/trade accept");
                        return;
                    }
                    if (!found)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Игрок {secondColor}{args[0]}{mainColor} не найден на сервере!");
                        return;
                    }
                }
                return;
            }
            if (args.Length == 2 || args.Length == 3)
            {
                if (args[0] == "item")
                {
                    UserInfo userInfo = null;
                    foreach (UserInfo info in tradeData.UserInfo)
                    {
                        if (info.steamID == UserID)
                        {
                            userInfo = info;
                        }
                    }
                    if (netUser.playerClient.controllable == null)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы {secondColor}не можете {mainColor}устанавливать предмет пока вы мертвы!");
                        return;
                    }
                    if (userInfo == null || userInfo.deal == null)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Сначала {secondColor}предложите {mainColor}игроку обмен предметами: {secondColor}/trade \"ник\"");
                        return;
                    }
                    if (!userInfo.deal.accepted)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Сначало нужно чтобы {secondColor}оба {mainColor}игрока приняли запрос на обмен!");
                        return;
                    }
                    if (userInfo.deal.leaderConfirmed || userInfo.deal.traderConfirmed)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}После подтверждения {secondColor}одного {mainColor}из участников сделки, предметы менять нельзя!");
                        return;
                    }
                    string itemName = args[1];
                    int count = 1;
                    if (args.Length == 3)
                    {
                        try
                        {
                            count = Int32.Parse(args[2]);
                        }
                        catch
                        {
                            rust.SendChatMessage(netUser, chatName, $"{mainColor}{args[2]} {secondColor}не {mainColor}является целым числом!");
                            return;
                        }
                    }
                    if (GetDataBlock(itemName) == null || itemName == "Torch")
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Предмет \"{secondColor}{itemName}{mainColor}\" не существует!");
                        return;
                    }
                    ItemDataBlock itemDataBlock = GetDataBlock(itemName);
                    if (count > itemDataBlock._maxUses)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{secondColor}{args[2]} слишком большое количество!");
                        return;
                    }
                    if (count <= 0)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{secondColor}{args[2]} неправильное количество!");
                        return;
                    }
                    if (itemDataBlock is WeaponDataBlock && count > 1)
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Оружие можно передавать только по {secondColor}1 {mainColor}штуке!");
                        return;
                    }
                    Inventory inv = netUser.playerClient.controllable.GetComponent<Inventory>();
                    int haveCount = 0;                   
                    inv.FindItem(itemDataBlock, out haveCount);
                    if (haveCount < count && !(itemDataBlock is WeaponDataBlock))
                    {
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}У вас в инвентаре не хватает {secondColor}{count - haveCount}{mainColor} x {secondColor}{itemName}");
                        return;
                    }
                    IInventoryItem item = GetItemInInventory(inv, itemDataBlock, count);
                    if (item == null)
                    {                       
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Предметы, которые вы предлагаете должны занимать {secondColor}1 {mainColor}слот!");
                        return;
                    }
                    NetUser netTarget = null;
                    if (userInfo.deal.leaderId == netUser.userID)
                    {
                        netTarget = NetUser.FindByUserID(userInfo.deal.traderId);
                        userInfo.deal.leaderCount = count;
                        userInfo.deal.leaderItem = item;

                        if (userInfo.deal.traderItem == null)
                            rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы предлагаете {secondColor}{count}{mainColor} x {secondColor}{itemName}");
                    }
                    if (userInfo.deal.traderId == netUser.userID)
                    {
                        netTarget = NetUser.FindByUserID(userInfo.deal.leaderId);
                        userInfo.deal.traderCount = count;
                        userInfo.deal.traderItem = item;
                        if (userInfo.deal.leaderItem == null)
                         rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы предлагаете {secondColor}{count}{mainColor} x {secondColor}{itemName}");
                    }             
                    

                    if (userInfo.deal.leaderItem != null && userInfo.deal.traderItem != null)
                    {
                        IInventoryItem itemGet = null;
                        int getCount = 0;
                        if (userInfo.deal.traderId == netUser.userID)
                        {
                            getCount = userInfo.deal.leaderCount;
                            itemGet = userInfo.deal.leaderItem;
                        } else
                        {
                            getCount = userInfo.deal.traderCount;
                            itemGet = userInfo.deal.traderItem;
                        }

                      //rust.SendChatMessage(netUser, chatName, $"{secondColor}Обмен предметами почти завершен:");
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы отдаете: {secondColor}{count}{mainColor} x {secondColor}{itemName}");
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Вы получаете: {secondColor}{getCount}{mainColor} x {secondColor}{itemGet.datablock.name}");
                        rust.SendChatMessage(netUser, chatName, $"{mainColor}Подтвердить: {secondColor}/trade confirm{mainColor}, отменить: {secondColor}/trade cancel");
                       
                      //rust.SendChatMessage(netTarget, chatName, $"{secondColor}Обмен предметами почти завершен:");
                        rust.SendChatMessage(netTarget, chatName, $"{mainColor}Вы отдаете: {secondColor}{getCount}{mainColor} x {secondColor}{itemGet.datablock.name}");
                        rust.SendChatMessage(netTarget, chatName, $"{mainColor}Вы получаете: {secondColor}{count}{mainColor} x {secondColor}{itemName}");
                        rust.SendChatMessage(netTarget, chatName, $"{mainColor}Подтвердить: {secondColor}/trade confirm{mainColor}, отменить: {secondColor}/trade cancel");
                       
                    }
                }
            }
        }

        void FinishTrade(DealInfo deal)
        {
            if (deal.Confirmed())
            {
                NetUser netLeader = null, netTrader = null;
                foreach (PlayerClient current in PlayerClient.All)
                if (current != null)
                {
                    if (current.userID == deal.leaderId)
                        netLeader = current.netUser;

                    if (current.userID == deal.traderId)
                        netTrader = current.netUser;
                }

                if (netLeader == null || netTrader == null || netLeader.playerClient.controllable.character.dead || netTrader.playerClient.controllable.character.dead)
                {
                    if (netLeader == null && netTrader != null)
                        rust.SendChatMessage(netTrader, chatName, $"{mainColor}Сделка с игроком была {secondColor}отменена {mainColor}из-за отсутствия игрока на сервере!");

                    if (netTrader == null && netLeader != null)
                        rust.SendChatMessage(netLeader, chatName, $"{mainColor}Сделка с игроком была {secondColor}отменена {mainColor}из-за отсутствия игрока на сервере!");
                }
                else
                {
                    Inventory leaderInv = netLeader.playerClient.controllable.GetComponent<Inventory>();
                    Inventory traderInv = netTrader.playerClient.controllable.GetComponent<Inventory>();
                    bool success1 = false, success2 = false;
                    bool canremove = false;

                    if (GetItemInInventory(leaderInv, deal.leaderItem.datablock, deal.leaderItem.uses) != null)
                    {
                        if (GetItemInInventory(traderInv, deal.traderItem.datablock, deal.traderItem.uses) != null)
                            canremove = true;
                    }

                    if (canremove && leaderInv.RemoveItem(deal.leaderItem)) // Если игрок лидер
                    {
                        success1 = true;
                    }
                    else
                    {
                        rust.SendChatMessage(netLeader, chatName, $"{mainColor}Сделка с игроком была {secondColor}отменена {mainColor}из-за отсутствия предмета у вас в инвентаре!");
                        rust.SendChatMessage(netTrader, chatName, $"{mainColor}Сделка с игроком была {secondColor}отменена {mainColor}из-за отсутствия предмета у игрока в инвентаре!");
                    }

                    if (canremove && traderInv.RemoveItem(deal.traderItem)) //Если игрок трейдер
                    {
                        success2 = true;
                    }
                    else
                    {
                        rust.SendChatMessage(netTrader, chatName, $"{mainColor}Сделка с игроком была {secondColor}отменена {mainColor}из-за отсутствия предмета у вас в инвентаре!");
                        rust.SendChatMessage(netLeader, chatName, $"{mainColor}Сделка с игроком была {secondColor}отменена {mainColor}из-за отсутствия предмета у игрока в инвентаре!");
                    }

                    if (success1 && success2)
                    {
                        int slot = GetFreeSlot(leaderInv);
                        if (slot != -1)
                        {
                            AddItem(leaderInv, slot, deal.traderItem.datablock, deal.traderItem.uses);
                            IInventoryItem gettedItem;
                            if (leaderInv.GetItem(slot, out gettedItem))
                            {
                                var oldItem = deal.traderItem as IHeldItem;
                                gettedItem.SetCondition(Convert.ToSingle(deal.traderItem.condition));
                                gettedItem.SetUses(deal.traderItem.uses);

                                if (gettedItem.datablock is WeaponDataBlock)
                                {
                                    if (Convert.ToInt32(oldItem.totalModSlots) != 0)
                                    {
                                        var m = gettedItem as IHeldItem;
                                        m.SetTotalModSlotCount(Convert.ToInt32(oldItem.totalModSlots));
                                        Dictionary<string, string> fixNameMod = new Dictionary<string, string>();
                                        fixNameMod.Add("Laser", "Laser Sight");
                                        fixNameMod.Add("Lamp", "Flashlight Mod");
                                        fixNameMod.Add("Sight", "Holo sight");
                                        fixNameMod.Add("Audio", "Silencer");
                                        fixNameMod.Add("Other", "");
                                        string mods = oldItem.modFlags.ToString();
                                        string[] nameMod = mods.Split(',');
                                        foreach (var d in nameMod)
                                        {
                                            ItemDataBlock byName = DatablockDictionary.GetByName(fixNameMod[d.Trim()]);
                                            if (byName is ItemModDataBlock)
                                            {
                                                m.AddMod(byName as ItemModDataBlock);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (success1 && success2)
                    {
                        int slot = GetFreeSlot(traderInv);
                        if (slot != -1)
                        {
                            AddItem(traderInv, slot, deal.leaderItem.datablock, deal.leaderItem.uses);
                            IInventoryItem gettedItem;
                            if (traderInv.GetItem(slot, out gettedItem))
                            {
                                var oldItem = deal.leaderItem as IHeldItem;
                                gettedItem.SetCondition(Convert.ToSingle(deal.leaderItem.condition));
                                gettedItem.SetUses(deal.leaderItem.uses);
                                if (gettedItem.datablock is WeaponDataBlock)
                                {
                                    if (Convert.ToInt32(oldItem.totalModSlots) != 0)
                                    {
                                        var m = gettedItem as IHeldItem;
                                        m.SetTotalModSlotCount(Convert.ToInt32(oldItem.totalModSlots));
                                        Dictionary<string, string> fixNameMod = new Dictionary<string, string>();
                                        fixNameMod.Add("Laser", "Laser Sight");
                                        fixNameMod.Add("Lamp", "Flashlight Mod");
                                        fixNameMod.Add("Sight", "Holo sight");
                                        fixNameMod.Add("Audio", "Silencer");
                                        fixNameMod.Add("Other", "");
                                        string mods = oldItem.modFlags.ToString();
                                        string[] nameMod = mods.Split(',');
                                        foreach (var d in nameMod)
                                        {
                                            ItemDataBlock byName = DatablockDictionary.GetByName(fixNameMod[d.Trim()]);
                                            if (byName is ItemModDataBlock)
                                            {
                                                m.AddMod(byName as ItemModDataBlock);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (success1 && success2)
                    {
                        rust.SendChatMessage(netLeader, chatName, $"{successColor}Сделка с игроком была успешно завершена!");
                        rust.InventoryNotice(netLeader, $"{deal.traderCount} x {deal.traderItem.datablock.name}");
                        //rust.SendChatMessage(netLeader, chatName, $"{mainColor}Вы получили {secondColor}{deal.traderCount}{mainColor} x {deal.traderItem.datablock.name}");
                        //rust.SendChatMessage(netLeader, chatName, $"{mainColor}Вы отдали {secondColor}{deal.leaderCount}{mainColor} x {secondColor}{deal.leaderItem.datablock.name}");

                        rust.SendChatMessage(netTrader, chatName, $"{successColor}Сделка с игроком была успешно завершена!");
                        rust.InventoryNotice(netTrader, $"{deal.leaderCount} x {deal.leaderItem.datablock.name}");
                        //rust.SendChatMessage(netTrader, chatName, $"{mainColor}Вы получили {secondColor}{deal.leaderCount}{mainColor} x {deal.leaderItem.datablock.name}");
                        //rust.SendChatMessage(netTrader, chatName, $"{mainColor}Вы отдали {secondColor}{deal.traderCount}{mainColor} x {secondColor}{deal.traderItem.datablock.name}");
                    }
                }
                deal.Remove();
                return;
            } // end: if (deal.Confirmed()) {}
        }
        
    }
}