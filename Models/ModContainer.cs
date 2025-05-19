using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DatRepacker.Models
{
    public class ModContainer
    {
        public short type { private set; get; } // 0: Folder, 1: Dat file
        public string name { private set; get; }
        public uint priority { private set; get; }

        public ModContainer(short type, string name, uint priority)
        {
            this.type = type;
            this.name = name;
            this.priority = priority;
        }
    }

    public class DatContainer: ModContainer
    {
        public BayoDat dat { private set; get; }

        public DatContainer(BayoDat dat, string name, uint priority) : base(1, name, priority)
        {
            this.dat = dat;
        }

        public bool IsBigEndian()
        {
            return dat.bigEndian;
        }
    }

    public class FolderContainer : ModContainer
    {
        public string path { private set; get; }

        public FolderContainer(string path, string name, uint priority) : base(0, name, priority)
        {
            this.path = path;
        }
    }
}
