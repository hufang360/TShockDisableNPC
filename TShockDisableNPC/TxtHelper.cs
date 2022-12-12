using System.Collections.Generic;
using Terraria.DataStructures;

namespace DisableNPC
{

    #region XMLHelper
    public class TxtHelper
    {
        private static List<TileProperty> tiles = new();
        private static bool isLoad = false;

        public static void Load()
        {
            if (isLoad) return;
            else isLoad = true;

            foreach (string line in utils.FromEmbeddedPath("DisableNPC.res.TileFrameData.txt").Split('\n'))
            {
                var arr = line.Split(';');

                var arrSize = arr[1].Split(',');
                var arrFrame = arr[2].Split(':');

                TileProperty tile = new()
                {
                    id = int.Parse(arr[0])
                };
                if (arrSize.Length == 2)
                {
                    tile.w = int.Parse(arrSize[0]);
                    tile.h = int.Parse(arrSize[1]);
                }
                foreach (string s in arrFrame)
                {
                    var arrUV = s.Split(',');
                    if (arrUV.Length == 2)
                    {
                        tile.frames.Add(new Point16(int.Parse(arrUV[0]), int.Parse(arrUV[1])));
                    }
                }
                tiles.Add(tile);
            }

            //var xmlSettings = XElement.Parse(FromEmbeddedPath("DisableNPC.res.settings.xml"));
            //foreach (var xElement in xmlSettings.Elements("Tiles").Elements("Tile"))
            //{
            //    TileProperty tile = new TileProperty();

            //    tile.Id = (int?)xElement.Attribute("Id") ?? 0;
            //    tile.IsFramed = (bool?)xElement.Attribute("Framed") ?? false;

            //    Point16 p = StringToPoint16((string)xElement.Attribute("Size"));
            //    if (p.X != 0 && p.Y != 0)
            //    {
            //        tile.Width = p.X;
            //        tile.Height = p.Y;
            //    }


            //    StringBuilder text = new StringBuilder();
            //    foreach (var elementFrame in xElement.Elements("Frames").Elements("Frame"))
            //    {
            //        p = StringToPoint16((string)elementFrame.Attribute("UV"));
            //        tile.frames.Add(p);
            //        text.Append(":"+(string)elementFrame.Attribute("UV"));
            //    }
            //    TShock.Log.Info($"{tile.Id};{(string)xElement.Attribute("Size")};{text}");

            //    tiles.Add(tile);
            //}
        }

        //private static Point16 StringToPoint16(string text)
        //{
        //    Point16 p = new Point16();
        //    if (!string.IsNullOrWhiteSpace(text))
        //    {
        //        var split = text.Split(',');
        //        if (split.Length == 2)
        //        {
        //            if (short.TryParse(split[0], out short x) && short.TryParse(split[1], out short y))
        //            {
        //                return new Point16(x, y);
        //            }
        //        }
        //    }
        //    return p;
        //}


        public static FindData GetFindData(TileLiteData p)
        {
            FindData fd = new(p.id, p.style);
            fd.style = p.style;

            TileProperty tprop = GetTileProp(p.id);
            fd.w = tprop.w;
            fd.h = tprop.h;
            if (p.style != -1 && p.style < tprop.frames.Count)
            {
                fd.frameX = tprop.frames[p.style].X;
                fd.frameY = tprop.frames[p.style].Y;
            }
            return fd;
        }

        private static TileProperty GetTileProp(int id)
        {
            foreach (var tile in tiles)
            {
                if (tile.id == id) return tile;
            }
            return new TileProperty();
        }
    }
    #endregion


    #region TileProperty
    public class TileProperty
    {
        public int id = 0;
        public int w = 1;
        public int h = 1;
        public List<Point16> frames = new();
    }
    #endregion




    #region FindData
    public class FindData
    {
        public int id = 0;
        public int w = 1;
        public int h = 1;
        public int style = -1;
        public int frameX = -1;
        public int frameY = -1;

        public FindData()
        {
        }

        public FindData(int _id, int _style = 0, int _w = 1, int _h = 1, int _frameX = -1, int _frameY = -1)
        {
            id = _id;
            w = _w;
            h = _h;
            style = _style;
            frameX = _frameX;
            frameY = _frameY;
        }

        public override string ToString()
        {
            return $"id={id},style={style},w={w},h={h}";
        }
    }
    #endregion
}
