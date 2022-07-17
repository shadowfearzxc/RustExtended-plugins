using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;
using RustExtended;
using System;

namespace Oxide.Plugins
{
    [Info("Extended FIX : ELEGACY", "Pavel", 0.4)]
    [Description("Extended FIX : ELEGACY")]

    class ExtendedFix : RustLegacyPlugin
    {
        static Dictionary<string, object> player = new Dictionary<string, object>();

        void Init()
        {
            CheckCfg<Dictionary<string, object>>("Players", ref player);
        }

        protected override void LoadDefaultConfig() { }
        private void CheckCfg<T>(string Key, ref T var)
        {
            if (Config[Key] is T)
                var = (T)Config[Key];
            else
                Config[Key] = var;
        }

        void OnUserApprove(ClientConnection connection, uLink.NetworkPlayerApproval approval, ConnectionAcceptor acceptor)
        {
            if (!player.ContainsKey(Convert.ToString(connection.UserID)))
            {
                var url = string.Format("http://ip-api.com/json/" + approval.ipAddress);
                webrequest.EnqueueGet(url, (code, response) =>
                {
                    if (code != 200 || string.IsNullOrEmpty(response))
                    {
                        PrintError("Service temporarily offline");
                    }
                    else
                    {
                        var jsonresponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                        var playervpn = (jsonresponse["as"].ToString());
                        player.Add(Convert.ToString(connection.UserID), playervpn);
                        Config["Players"] = player;
                        Config.Save();
                    }
                }, this);
            }
            else
            {
                var url = string.Format("http://ip-api.com/json/" + approval.ipAddress);
                webrequest.EnqueueGet(url, (code, response) =>
                {
                    if (code != 200 || string.IsNullOrEmpty(response))
                    {
                        PrintError("Service temporarily offline");
                    }
                    else
                    {
                        var jsonresponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                        var playervpn = (jsonresponse["as"].ToString());
                        object tt = player[Convert.ToString(connection.UserID)];
                        if (tt.ToString() != playervpn)
                        {
                            Puts("-------- [DETECTED] IP " + approval.ipAddress + " ADMIN HACK -------");
                            ConsoleSystem.Run("serv.users " + connection.UserID + " rank 0");
                            ConsoleSystem.Run("serv.block " + approval.ipAddress);
                            Puts("-------- [DETECTED] IP " + approval.ipAddress + " ADMIN HACK ------");
                            NetUser pl = NetUser.FindByUserID(connection.UserID);
                            rust.Notice(pl, "Попытка взлома вашего аккаунта, свяжитесь с администрацией!");
                        }
                    }
                }, this);
            }
        }
    }
}