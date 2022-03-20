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
        public DateTime dt;

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
                    if (version >= 2)
                    {
                        // wantMini
                        state.wantMini = fs.ReadByte() != 0;
                    }
                    if (version == 3)
                    {
                        // date
                        int count = fs.Read(buffer, 0, 6);
                        if (count == 6)
                            state.dt = new DateTime(2000 + buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5]);
                        else
                            throw new Exception("bad settings file");
                    }
                    else
                        state.dt = new DateTime(2000, 1, 1, 0, 0, 5);
                }
            }

            return state;
        }

        public void saveSettings()
        {
            using (FileStream fs = File.Open(PATH_SETTINGS, FileMode.Create))
            {
                fs.WriteByte(3); // version
                fs.Write(BitConverter.GetBytes(MAC), 0, 6);
                fs.Write(BitConverter.GetBytes(magic), 0, 4);
                fs.WriteByte(wantMini ? (byte)1 : (byte)0);

                fs.WriteByte((byte)(dt.Year - 2000));
                fs.WriteByte((byte)(dt.Month));
                fs.WriteByte((byte)(dt.Day));
                fs.WriteByte((byte)(dt.Hour));
                fs.WriteByte((byte)(dt.Minute));
                fs.WriteByte((byte)(dt.Second));
            }
        }


    }
}
