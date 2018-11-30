using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DTcms.Model
{
    public class sotr_order
    {
        public int id;
        public int sort_id = 99;
    }

    public class SheetSong
    {
        public int id;
        public string songId;
        public string name;
        public string type;
        public string albumName;
        public string artistName;
        public string albumCover;
        public string lyric;
        public int taxis;
    }
}