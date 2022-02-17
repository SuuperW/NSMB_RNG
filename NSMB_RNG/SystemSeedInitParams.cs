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
        public int Seconds;
        public bool Is3DS;

        private SystemSeedInitParams(uint magic)
        {
            Timer0 =  (ushort)((magic >>  0) & 0xfff);
            VCount =  (ushort)((magic >> 12) & 0x1ff);
            VFrame =  (magic >> 21) & 0xf;
            Seconds = (int)((magic >> 25) & 0x3f);
            Is3DS =   ((magic >> 31) & 0x1) == 1;
        }

        public static uint GetMagic(SystemSeedInitParams seedParams, uint seconds = 0)
        {
            uint magic = 0;
            // Verify
            if (seedParams.Timer0 > 0xfff || seedParams.VCount > 0x1ff || seedParams.VFrame > 0xf || seconds > 0x3f)
                return magic;

            magic = magic | ((uint)seedParams.Timer0 << 0);
            magic = magic | ((uint)seedParams.VCount << 12);
            magic = magic | (seedParams.VFrame << 21);
            magic = magic | (seconds << 25);
            magic = magic | ((seedParams.Is3DS ? 1u : 0u) << 0);
            return magic;
        }

        //public static SystemSeedInitParams DeSmuME_Personal = new SystemSeedInitParams(0x518, 0x28, 5);
        //public static SystemSeedInitParams BizHawk = new SystemSeedInitParams(0x41E, 0xF7, 4);
        public static uint DeSmuME_Personal = 0x00A28518;
        public static uint BizHawk = 0x008F741E;
    }
}
