namespace NSMB_RNG
{
    public class SystemSeedInitParams
    {
        public ushort Timer0;
        public ushort VCount;
        public uint VFrame;
        public bool Is3DS;

        public SystemSeedInitParams(uint magic)
        {
            Timer0 = (ushort)((magic >>  0) & 0xfff);
            Timer0 = (ushort)(Timer0 | ((magic >> 16) & 0x3000)); // It's split for backwards compatibility.
            VCount = (ushort)((magic >> 12) & 0x1ff);
            VFrame = (magic >> 24) & 0xf;
            Is3DS =  ((magic >> 31) & 0x1) == 1;
        }

        public static uint GetMagic(SeedInitParams seedParams)
        {
            uint magic = 0;
            // Verify
            if (seedParams.Timer0 > 0x3fff || seedParams.VCount > 0x1ff || seedParams.VFrame > 0xf)
                return magic;


            magic = magic | (uint)((seedParams.Timer0 & 0xfff) << 0);
            magic = magic | (uint)((seedParams.Timer0 & 0x3000) << 16); // It's split for backwards compatibility.
            magic = magic | ((uint)seedParams.VCount << 12);
            magic = magic | (seedParams.VFrame << 24);
            magic = magic | ((seedParams.Is3DS ? 1u : 0u) << 31);
            return magic;
        }

        public void SetSeedParams(SeedInitParams seedParams)
        {
            seedParams.Timer0 = Timer0;
            seedParams.VCount = VCount;
            seedParams.VFrame = VFrame;
            seedParams.Is3DS = Is3DS;
        }

        //public static SystemSeedInitParams DeSmuME_Personal = new SystemSeedInitParams(0x518, 0x28, 5);
        //public static SystemSeedInitParams BizHawk = new SystemSeedInitParams(0x41E, 0xF7, 4);
        public static uint DeSmuME_Personal = 0x05028518;
        public static uint BizHawk = 0x040F741E;
    }
}
