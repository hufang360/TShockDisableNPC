using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using TShockAPI;


namespace DisableNPC
{
    public class utils
    {
        public static int GetUnixTimestamp { get { return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; } }

        public static string FromEmbeddedPath(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        public static void updatePlayerSlot(TSPlayer player, Item item, int slotIndex)
        {
            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(item.Name), player.Index, slotIndex, (float)item.prefix);
            NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(item.Name), player.Index, slotIndex, (float)item.prefix);
        }


        public static void Log(string msg) { TShock.Log.ConsoleInfo("[禁npc]" + msg); }
    }


}
