using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;


namespace DisableNPC
{
    public class TileHelper
    {
        public static bool isTaskRunning = false;
        public static int clearCount = 0;

        public static async void AsyncClear(TSPlayer op, List<TileLiteData> targets, List<int> ids)
        {
            if (isTaskRunning)
            {
                op.SendInfoMessage("有清理任务正在进行，请稍后再试");
                return;
            }
            int startTime = utils.GetUnixTimestamp;
            await Task.Run(() =>
            {
                ClearTile(targets, ids);
            }).ContinueWith((d) =>
            {
                FinishGen(op != null);
                InformPlayers();

                int seconds = utils.GetUnixTimestamp - startTime;
                if (op == null)
                    utils.Log($"已清除 {clearCount} 个目标，耗时 {seconds}s ！");
                else
                    op.SendInfoMessage($"已清除 {clearCount} 个目标，耗时 {seconds}s ！");
            });
        }


        private static void ClearTile(List<TileLiteData> targets, List<int> ids)
        {
            ResetSkip();
            isTaskRunning = true;

            int count = 0;
            foreach (TileLiteData p in targets)
            {
                FindData fd = TxtHelper.GetFindData(p);
                //Console.WriteLine($"start {fd.id}：{fd.w},{fd.h} frame：{fd.frameX},{fd.frameY}");
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    for (int y = 0; y < Main.maxTilesY; y++)
                    {
                        ITile tile = Main.tile[x, y];
                        if (tile.active() && tile.type == fd.id)
                        {
                            if (GetItem(x, y, fd))
                            {
                                ClearTile(x, y, fd.w, fd.h);
                                count++;
                            }
                        }
                    }
                }
            }

            foreach (Chest ch in Main.chest.Where(ch => ch != null))
            {
                for (int k = 0; k < 40; k++)
                {
                    Item item = ch.item[k];
                    if (item == null) continue;
                    if (!item.active) continue;
                    if (ids.Contains(item.netID))
                    {
                        ch.item[k].active = false;
                        ch.item[k].netID = 0;
                        NetMessage.SendData(31, -1, -1, null, ch.x, ch.y);
                        count++;
                    }
                }
            }

            isTaskRunning = false;
            clearCount = count;
        }

        private static void ClearTile(int x, int y, int w, int h)
        {
            for (int i = x; i < x + w; i++)
            {
                for (int k = y; k < y + h; k++)
                {
                    Main.tile[i, k].type = 0;
                    Main.tile[i, k].active(false);
                    Main.tile[i, k].slope(0);
                    Main.tile[i, k].halfBrick(false);
                }
            }
        }


        public static void FinishGen(bool needSound = true)
        {
            if (needSound)
            {
                foreach (TSPlayer player in TShock.Players)
                {
                    if (player != null && (player.Active))
                    {
                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(player.TPlayer.position, 1, 0, 10, -16));
                    }
                }
            }
            TShock.Utils.SaveWorld();
        }

        public static void InformPlayers()
        {
            foreach (TSPlayer person in TShock.Players)
            {
                if ((person != null) && (person.Active))
                {
                    for (int i = 0; i < 255; i++)
                    {
                        for (int j = 0; j < Main.maxSectionsX; j++)
                        {
                            for (int k = 0; k < Main.maxSectionsY; k++)
                            {
                                Netplay.Clients[i].TileSections[j, k] = false;
                            }
                        }
                    }
                }
            }
        }

        public static bool GetItem(int tileX, int tileY, FindData fd)
        {
            ITile tile = Main.tile[tileX, tileY];
            int frameX = tile.frameX;
            int frameY = tile.frameY;
            int type = tile.type;
            //Console.WriteLine($"GetItem id={fd.id} style={fd.style} wh={fd.w},{fd.h} frame={fd.frameX},{fd.frameY}");
            bool check(int w, int h)
            {
                bool pass = true;
                for (int x = tileX; x < tileX + w; x++)
                {
                    for (int y = tileY; y < tileY + h; y++)
                    {
                        if (Main.tile[x, y].type != type || !Main.tile[x, y].active() || ContainsSkip(x, y))
                        {
                            pass = false;
                            break;
                        }
                    }
                }
                if (pass)
                {
                    skip.Add(new Rectangle(tileX, tileY, w, h));
                    //Console.WriteLine($"type={26}：{tileX},{tileY} frame：{frameX},{frameY}");
                }
                return pass;
            }

            bool flag = frameX == fd.frameX ? true : false;
            bool flag2 = frameY == fd.frameY ? true : false;
            if (fd.style == -1)
            {
                flag = true;
                flag2 = true;
            }

            if (flag && flag2 && check(fd.w, fd.h))
            {
                return true;
            }
            return false;
        }
        private static List<Rectangle> skip = new();
        private static void ResetSkip() { skip.Clear(); }
        private static bool ContainsSkip(int x, int y)
        {
            foreach (Rectangle r in skip) { if (r.Contains(x, y)) return true; }
            return false;
        }

    }


}
