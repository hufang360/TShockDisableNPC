using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace DisableNPC
{
    [ApiVersion(2, 1)]
    public class DisableNPC : TerrariaPlugin
    {

        # region Plugin Info
        public override string Name => "禁NPC";
        public override string Description => "禁NPC、物品、放置物";
        public override string Author => "hufang360";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        #endregion

        private Config _config;
        private static string saveDir = Path.Combine(TShock.SavePath, "DisableNPC");
        private static string saveFile = Path.Combine(saveDir, "config.json");

        private bool hasPlayer = false;
        private List<string> muteMsgs = new List<string>();

        private int LastTime = 0;

        private string Permission = "disablenpc";

        public DisableNPC(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(DNCommand, "disablenpc", "dn") { HelpText = "禁NPC" });

            if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

            Reload();
            ServerApi.Hooks.NpcSpawn.Register(this, OnNpcSpawn, 1);
            ServerApi.Hooks.ServerBroadcast.Register(this, OnBroadcast, 1);
            ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin, 5);
            GetDataHandlers.PlayerSlot.Register(OnPlayerSlot, HandlerPriority.Highest);
            GetDataHandlers.PlaceObject.Register(OnPlaceObject, HandlerPriority.Highest);
            GetDataHandlers.ItemDrop.Register(OnItemDrop);
        }

        private void DNCommand(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0)
            {
                op.SendInfoMessage("用法不对，输入 /dn help 查看帮助");
                return;
            }

            void ShowHelpText()
            {
                op.SendInfoMessage("/dn wof，召唤血肉墙");
                op.SendInfoMessage("/dn ske，召唤骷髅王");
                op.SendInfoMessage("/dn altar，模拟打破祭坛");
                if (op.HasPermission(Permission))
                {
                    op.SendInfoMessage("/dn clear，清理放置物");
                    op.SendInfoMessage("/dn reload，重载配置");
                }
            }

            int curTime = GetUnixTimestamp;
            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "help": ShowHelpText(); return;
                default: op.SendInfoMessage("用法不对，输入 /dn help 查看帮助"); return;

                case "ske":
                    if (curTime - LastTime > 180)
                    {
                        if (!op.RealPlayer)
                        {
                            op.SendErrorMessage("需要在游戏内操作!");
                            return;
                        }
                        foreach (NPC npc in Main.npc)
                        {
                            if (npc.active && npc.netID == 37)
                            {
                                op.SendErrorMessage("骷髅王已存在!");
                                return;
                            }
                        }
                        if (NPC.downedBoss3)
                        {
                            op.SendErrorMessage("骷髅王已被击败，无法再次召唤!");
                            return;
                        }
                        if (Main.dayTime)
                        {
                            op.SendErrorMessage("只能在夜晚召唤!");
                            return;
                        }

                        Rectangle area = new Rectangle(op.TileX - 61, op.TileY - 34 + 3, 122, 68);
                        if (InArea(area, Main.dungeonX, Main.dungeonY))
                        {
                            NPC npc = new NPC();
                            npc.SetDefaults(35);
                            TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, 1, args.Player.TileX, args.Player.TileY);
                            LastTime = curTime;
                            TSPlayer.All.SendInfoMessage($"{op.Name} 召唤了骷髅王!");
                        }
                        else
                        {
                            op.SendErrorMessage("需要在地牢入口附近!");
                            return;
                        }
                    }
                    else
                    {
                        op.SendErrorMessage($"操作太快了，请3分钟后再操作!");
                    }
                    break;


                case "wof":
                    if (curTime - LastTime > 180)
                    {
                        if (Main.wofNPCIndex != -1)
                        {
                            op.SendErrorMessage("血肉墙已存在!");
                            return;
                        }
                        if (args.Player.Y / 16f < Main.maxTilesY - 205)
                        {
                            op.SendErrorMessage("血肉墙只能在地狱进行召唤!");
                            return;
                        }
                        if (!op.RealPlayer)
                        {
                            op.SendErrorMessage("需要在游戏内操作!");
                            return;
                        }

                        // 检查和减扣 向导巫毒娃娃
                        int index = -1;
                        for (int i = 0; i < 54; i++)
                        {
                            Item item = op.TPlayer.inventory[i];
                            if (item.active && item.netID == 267)
                            {
                                index = i;
                                item.netID = 0;
                                item.active = false;
                                break;
                            }
                        }
                        if (index == -1)
                        {
                            op.SendErrorMessage("你的背包里没有[i:267]向导巫毒娃娃!");
                            return;
                        }
                        utils.updatePlayerSlot(op, op.TPlayer.inventory[index], index);

                        // 召唤血肉墙
                        NPC.SpawnWOF(new Vector2(op.X, op.Y));
                        LastTime = curTime;
                        TSPlayer.All.SendInfoMessage($"{op.Name} 召唤了血肉墙!");
                    }
                    else
                    {
                        op.SendErrorMessage($"操作太快了，请3分钟后再操作!");
                    }
                    break;


                // 打破祭坛
                case "altar":
                    if (!Main.hardMode)
                    {
                        op.SendErrorMessage("需要先击败血肉墙！");
                        return;
                    }
                    if (WorldGen.altarCount >= 9)
                    {
                        op.SendInfoMessage($"已打破{WorldGen.altarCount}个祭坛，超过9个就不能使用本指令了!");
                        return;
                    }
                    WorldGen.SmashAltar(op.TileX, op.TileY);
                    op.SendSuccessMessage($"已打破{WorldGen.altarCount}个祭坛");
                    break;


                case "clear":
                    if (!op.HasPermission(Permission))
                    {
                        op.SendErrorMessage("你没有权限执行清理操作");
                        return;
                    }
                    TileHelper.AsyncClearTile(op, _config.tiles);
                    break;

                case "reload":
                    if (!op.HasPermission(Permission))
                    {
                        op.SendErrorMessage("你没有权限执行重载操作");
                        return;
                    }
                    Reload();
                    op.SendSuccessMessage("[禁npc]已重新加载配置");
                    break;
            }
        }

        private void Reload()
        {
            _config = Config.Load(saveFile);
        }

        // 检查NPC
        private void OnNpcSpawn(NpcSpawnEventArgs args)
        {
            NPC npc = Main.npc[args.NpcId];
            if (_config.npcList.Contains(npc.netID))
            {
                string name = npc.FullName;
                npc.active = false;
                npc.type = 0;
                TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", args.NpcId);
                args.Handled = true;
                muteMsgs.Add(Language.GetTextValue("Announcement.HasArrived", name));
            }
        }

        // 屏蔽NPC到达通知
        private void OnBroadcast(ServerBroadcastEventArgs args)
        {
            if (args.Handled) return;
            string msg = args.Message.ToString();
            if (muteMsgs.Contains(msg))
            {
                utils.Log(msg + "（在游戏里看不到这条消息）");
                muteMsgs.Remove(msg);
                args.Handled = true;
            }
        }


        // 第一个玩家进入世界
        private void OnServerJoin(JoinEventArgs args)
        {
            //TSPlayer op = TShock.Players[args.Who];

            // 第一个玩家进入世界时 执行检查
            if (!hasPlayer)
            {
                TxtHelper.Load();

                hasPlayer = true;

                // 清理NPC
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && _config.npcList.Contains(Main.npc[i].netID))
                    {
                        Main.npc[i].active = false;
                        Main.npc[i].type = 0;
                        TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
                    }
                }
                // 清理放置物
                TileHelper.AsyncClearTile(null, _config.tiles);
            }
        }

        // 检查玩家背包
        private void OnPlayerSlot(object sender, GetDataHandlers.PlayerSlotEventArgs args)
        {
            TSPlayer op = args.Player;
            short id = args.Type;
            short slot = args.Slot;

            if (_config.playerSlotCheck && _config.itemList.Contains(id))
            {
                Item item;
                if (slot < 54)
                    item = op.TPlayer.inventory[slot];
                else if (slot >= 99 && slot < 139)
                    item = op.TPlayer.bank.item[slot - 99];
                else if (slot >= 139 && slot < 179)
                    item = op.TPlayer.bank2.item[slot - 139];
                else if (slot >= 180 && slot < 220)
                    item = op.TPlayer.bank3.item[slot - 180];
                else if (slot >= 220 && slot < 260)
                    item = op.TPlayer.bank4.item[slot - 220];
                else
                    return;

                if (item != null)
                {
                    //string name = item.Name;
                    item.active = false;
                    item.netID = 0;
                    utils.updatePlayerSlot(op, item, slot);
                    //utils.Log($"[禁npc][i/s{args.Stack}:{id}]{name} 已被清除");
                }
            }
        }


        // 检查放置物
        private void OnPlaceObject(object sender, GetDataHandlers.PlaceObjectEventArgs args)
        {
            short x = args.X;
            short y = args.Y;
            short type = args.Type;
            short style = args.Style;

            foreach (TileLiteData td in _config.tiles.Where(td => td.id == type))
            {
                if (td.style == -1 || td.style == style)
                {
                    args.Handled = true;
                    FindData fd = TxtHelper.GetFindData(td);
                    TSPlayer.All.SendTileSquare(args.X, args.Y, Math.Max(fd.w, fd.h));
                    //args.Player.SendErrorMessage("[禁npc]此物品不允许放置");
                }
            }
        }

        private void OnItemDrop(object sender, GetDataHandlers.ItemDropEventArgs args)
        {
            short id = args.ID;
            short type = args.Type;
            short stack = args.Stacks;

            // 玩家扔出物品
            if (id == 400)
            {
                if (_config.itemList.Contains(type))
                {
                    utils.Log($"[禁npc][i/s{stack}:{type}] 已被清除");
                    args.Handled = true;
                    return;
                }
            }

            // 检查所有掉落物
            for (int i = 0; i < 400; i++)
            {
                Item item = Main.item[i];
                if (item.active && _config.itemList.Contains(item.netID))
                {
                    utils.Log($"[禁npc][i/s{item.stack}:{item.netID}] 已被清除");
                    item.active = false;
                    NetMessage.TrySendData(21, -1, -1, null, i);
                }
            }
        }
        private int GetUnixTimestamp { get { return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; } }
        private bool InArea(Rectangle rect, int x, int y) { return x >= rect.X && x <= rect.X + rect.Width && y >= rect.Y && y <= rect.Y + rect.Height; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnNpcSpawn);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnServerJoin);
                ServerApi.Hooks.ServerBroadcast.Deregister(this, OnBroadcast);

                GetDataHandlers.PlayerSlot.UnRegister(OnPlayerSlot);
                GetDataHandlers.PlaceObject.UnRegister(OnPlaceObject);
            }
            base.Dispose(disposing);
        }
    }

}
