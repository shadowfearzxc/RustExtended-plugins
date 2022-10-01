// Reference: Facepunch.MeshBatch
// Reference: Google.ProtocolBuffers
// Reference: Google.ProtocolBuffers
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Data;
using UnityEngine;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Core.Libraries.Covalence;
using RustProto;
using RustExtended;

namespace Oxide.Plugins
{
    [Info("VKStatus", "kustanovich", "1.0.0")]
    class VKStatus : RustLegacyPlugin
    {
		int groupID = 0;
		string IP = "127.0.0.1";
		int PORT = "27015";
		string token = "";
    	public static int online = 0;

    	static Plugins.Timer timerCheckControllable; 

        void Init()
        {
            Puts("[VKStatus PLUGIN] : VKStatus is loaded!");
            UpdateOnline();
        }

        void UpdateOnline()
        {
        	if (timerCheckControllable != null) 
            {
                timerCheckControllable.Destroy(); 
            }
        	timerCheckControllable = timer.Repeat(180f, 0, () => 
            { 
                online = 0;
                foreach (uLink.NetworkPlayer networkPlayer in NetCull.connections) 
                { 
                    NetUser netUser = networkPlayer.GetLocalData() as NetUser; 
                    if (netUser != null) 
                    { 
                        ++online;
                    } 
                }
                GetRequest();
            });
        }
        void GetRequest() 
        {
            webrequest.EnqueueGet("https://api.vk.com/method/status.set?text=Онлайн "+online+" из 200 игроков                 IP: net.connect " + IP + ":" + PORT + "&group_id=" + group_id + "&access_token=" + token + "&v=5.73", (code, response) => GetCallback(code, response), this);
        }

        void GetCallback(int code, string response)
        {
            if (response == null || code != 200)
            {
                return;
            }
        } 
    }
} 