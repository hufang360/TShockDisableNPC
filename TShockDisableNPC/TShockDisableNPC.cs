using System;
using System.Reflection;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Terraria.ID;


namespace Plugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {

        # region Plugin Info
        public override string Name => "禁NPC";
        public override string Description => "";
        public override string Author => "hufang360";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        #endregion

        private Config _config;
        private static string saveFilename = Path.Combine(TShock.SavePath, "DisableNPC.json");

        // 更新间隔
        private int total = 60;

        // 计时器
        private int count = 60;

        // private bool[] Deprecated;

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            // Deprecated =  ItemID.Sets.Deprecated;
            Reload();

            GeneralHooks.ReloadEvent += OnReload;
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        private void OnReload(ReloadEventArgs args)
        {
            Reload();
        }

        private void Reload()
        {
            _config = Config.Load(saveFilename);
            total = _config.second * 60;

            // ItemID.Sets.Deprecated = Deprecated;
            foreach (var id in _config.item)
            {
                ItemID.Sets.Deprecated[id] = true;
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if( count>0 )
            {
                count--;
                return;
            }

            count = total;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if( !Main.npc[i].active || Main.npc[i].type==0 || Main.npc[i].netID==0 )
                    continue;

                if( _config.npc.Contains(Main.npc[i].netID) )
                {
                    Main.npc[i].active = false;
                    Main.npc[i].type = 0;
                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
                }
            }
        }

        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                GeneralHooks.ReloadEvent -= OnReload;
			}
			base.Dispose(disposing);
		}
	}

}
