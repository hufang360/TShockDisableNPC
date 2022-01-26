using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Plugin
{
    public class Config
    {
        public List<int> npc = new List<int>();
        public List<int> item = new List<int>();

        // 每 1秒检查一次
        public int second = 2;

        public static Config Load(string path)
        {
            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
            }
            else
            {
                var c = new Config();
                c.InitDefault();
                File.WriteAllText(path, JsonConvert.SerializeObject(c, Formatting.Indented));
                return c;
            }
        }

        public void InitDefault()
        {
            npc = new List<int>(){
                // 22,          //向导
                // 369,         //渔夫  
                // 376,         //沉睡渔夫
                19,          //军火商
                54,          //服装商
                38,          //爆破专家
                20,          //树妖
                207,         //染料商
                107,         //哥布林工匠
                105,         //受缚哥布林
                588,         //高尔夫球手
                589,         //高尔夫球手救援
                124,         //机械师
                123,         //受缚机械师
                17,          //商人
                // 18,          //护士
                // 37,          //老人
                227,         //油漆工
                208,         //派对女孩
                453,         //骷髅商人
                353,         //发型师
                354,         //被网住的发型师
                550,         //酒馆老板
                579,         //昏迷男子
                368,         //旅商
                228,         //巫医
                633,         //动物学家
                209,         //机器侠
                229,         //海盗
                663,         //公主
                142,         //圣诞老人
                178,         //蒸汽朋克人
                // 441,         //税收官
                160,         //松露人
                108,         //巫师
                106,         //受缚巫师
                // 637,	        //城镇猫咪
                // 638,	        //城镇狗狗
                // 656,	        //城镇兔兔
            };

            item = new List<int>(){
                398,        // 工匠作坊
                363,        // 锯木机
                2172,        // 重型工作台
                332,        // 织布机
                3000,        // 炼药桌
                345,        // 烹饪锅
                1791,        // 大锅
                1120,        // 染缸
                
                35,        // 铁砧
                716,        // 铅砧
                525,        // 秘银砧
                1220,        // 山铜砧

                33,        // 熔炉
                221,        // 地狱熔炉
                524,        // 精金熔炉
                1221,        // 钛金熔炉

                487,        // 水晶球
                3549,        // 远古操纵机
                1551,        // 自动锤炼机


                36,        // 工作台
                4205,        // 星云工作台
                4163,        // 日耀工作台
                4226,        // 星旋工作台
                4184,        // 星旋工作台
                811,        // 骨头工作台
                4584,        // 竹工作台
                673,        // 针叶木工作台
                812,        // 仙人掌工作台
                3909,        // 水晶工作台
                2229,        // 王朝工作台
                635,        // 乌木工作台
                3158,        // 花岗岩工作台
                3157,        // 大理石工作台
                2826,        // 火星工作台
                3156,        // 陨石工作台
                814,        // 蘑菇工作台
                2534,        // 棕榈木工作台
                637,        // 珍珠木工作台
                1795,        // 南瓜工作台
                636,        // 红木工作台
                4315,        // 沙岩工作台
                916,        // 暗影木工作台
                815,        // 史莱姆工作台
                3949,        // 蜘蛛工作台
                1817,        // 阴森工作台
                3975,        // 病变工作台
                813,        // 血肉工作台
                2632,        // 玻璃工作台
                2251,        // 蜂蜜工作台
                2252,        // 冰冻工作台
                2633,        // 生命木工作台
                2631,        // 天域工作台
                815,        // 史莱姆工作台
                2253,        // 蒸汽朋克工作台
                1511,        // 哥特工作台
                1398,        // 蓝地牢工作台
                1401,        // 绿地牢工作台
                1404,        // 粉地牢工作台
                1145,        // 丛林蜥蜴工作台
                3910,        // 金工作台
                1461,        // 黑曜石工作台

                4900,        // 远古神圣板甲
                4901,        // 远古神圣护胫
                4896,        // 远古神圣面具
                4897,        // 远古神圣头盔
                4898,        // 远古神圣头饰
                4899,        // 远古神圣兜帽
                
                1331,        // 血腥脊椎
                560,        // 史莱姆王冠
                560,        // 虚空袋
                4076,        // 虚空保险库
                70,        // 蠕虫诱饵

                1133,        // 憎恶之蜂
                2896,        // 粘性雷管
                235,        // 粘性炸弹
                949,        // 雪球
                4423,        // 甲虫炸弹
                985,        // 绳圈
                1310,        // 毒镖
                3011,        // 灵液镖
                3009,        // 水晶镖
                3010,        // 诅咒镖
                5012,        // 烈焰链锤
                109,        // 魔力水晶
            };

        }

    }
}