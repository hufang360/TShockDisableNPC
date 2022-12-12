using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


namespace DisableNPC
{
    public class Config
    {
        public List<LiteData> npc = new();
        public List<LiteData> item = new();

        public List<TileLiteData> tiles = new();

        [JsonIgnore]
        public List<int> npcList = new();
        [JsonIgnore]
        public List<int> itemList = new();


        public static Config Load(string path)
        {
            Config c;
            string text;
            if (File.Exists(path))
            {
                text = File.ReadAllText(path);
            }
            else
            {
                // 读取内嵌配置文件
                text = utils.FromEmbeddedPath("DisableNPC.res.config.json");

                // 将内嵌配置文件拷出
                File.WriteAllText(path, text);
            }

            c = JsonConvert.DeserializeObject<Config>(text, new JsonSerializerSettings()
            {
                Error = (sender, error) => error.ErrorContext.Handled = true
            });

            // 补齐字段
            foreach (LiteData d in c.npc)
            {
                c.npcList.Add(d.id);
            }
            foreach (LiteData d in c.item)
            {
                c.itemList.Add(d.id);
            }

            return c;
        }

    }

    public class TileLiteData
    {
        public int id = 0;
        public int style = 0;
        public TileLiteData(int _id, int _style)
        {
            id = _id;
            style = _style;
        }
    }
    public class LiteData
    {
        public int id = 0;
    }
}