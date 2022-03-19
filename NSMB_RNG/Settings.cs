using System;
using System.IO;

namespace NSMB_RNG
{
    public class Settings
    {
        const string PATH_SETTINGS = "settings.bin";

        public ulong MAC = 0;
        public uint magic = 0;
        public bool wantMini = false;

        private Settings() { }

        public static Settings loadSettings()
        {
            Settings state = new Settings();

            if (File.Exists(PATH_SETTINGS))
            {
                using (FileStream fs = File.OpenRead(PATH_SETTINGS))
                {
                    byte[] buffer = new byte[8];
                    int version = fs.ReadByte();
                    if (version >= 1)
                    {
                        // MAC
                        int count = fs.Read(buffer, 0, 6);
                        if (count == 6)
                            state.MAC = BitConverter.ToUInt64(buffer);
                        // magic
                        count = fs.Read(buffer, 0, 4);
                        if (count == 4)
                            state.magic = BitConverter.ToUInt32(buffer);
                        else
                            throw new Exception("bad settings file");
                    }
                    if (version == 2)
                    {
                        // wantMini
                        state.wantMini = fs.ReadByte() != 0;
                    }
                }
            }

            return state;
        }

        public void saveSettings()
        {
            using (FileStream fs = File.Open(PATH_SETTINGS, FileMode.Create))
            {
                fs.WriteByte(2); // version
                fs.Write(BitConverter.GetBytes(MAC), 0, 6);
                fs.Write(BitConverter.GetBytes(magic), 0, 4);
                fs.WriteByte(wantMini ? (byte)1 : (byte)0);
            }
        }


    }
}
