using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSMB_RNG
{
    internal class SystemSeedInitParams
    {
        public ushort Timer0;
        public ushort VCount;
        public uint VFrame;

        private SystemSeedInitParams(ushort t, ushort c, uint f)
        {
            Timer0 = t;
            VCount = c;
            VFrame = f;
        }

        public static SystemSeedInitParams DeSmuME_Personal = new SystemSeedInitParams(0x518, 0x28, 5);
        public static SystemSeedInitParams BizHawk = new SystemSeedInitParams(1054, 247, 4);
    }
}
