using System;
using System.IO;
using System.Text;

namespace NSMB_RNG
{
    public class Settings
    {
        const string PATH_SETTINGS = "settings.bin";

        public ulong MAC = 0;
        public uint magic = 0;
        public bool wantMini = false;
        public DateTime dt;
        public string systemName = "";

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
                    if (version == 4)
                    {
                        // wantMini
                        state.wantMini = fs.ReadByte() != 0;
                        // date
                        int count = fs.Read(buffer, 0, 6);
                        if (count == 6)
                            state.dt = new DateTime(2000 + buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5]);
                        else
                            throw new Exception("bad settings file");
                        // systemName
                        int systemNameLength = fs.ReadByte();
                        if (systemNameLength > buffer.Length) buffer = new byte[systemNameLength];
                        count = fs.Read(buffer, 0, systemNameLength);
                        if (count == systemNameLength)
                            state.systemName = Encoding.ASCII.GetString(buffer, 0, systemNameLength);
                        else
                            throw new Exception("bad settings file");
                    }
                }
            }

            return state;
        }

        public void saveSettings()
        {
            using (FileStream fs = File.Open(PATH_SETTINGS, FileMode.Create))
            {
                fs.WriteByte(4); // version
                fs.Write(BitConverter.GetBytes(MAC), 0, 6);
                fs.Write(BitConverter.GetBytes(magic), 0, 4);
                fs.WriteByte(wantMini ? (byte)1 : (byte)0);

                fs.WriteByte((byte)(dt.Year - 2000));
                fs.WriteByte((byte)(dt.Month));
                fs.WriteByte((byte)(dt.Day));
                fs.WriteByte((byte)(dt.Hour));
                fs.WriteByte((byte)(dt.Minute));
                fs.WriteByte((byte)(dt.Second));

                if (systemName.Length > 50)
                    systemName = systemName.Substring(0, 50);
                fs.WriteByte((byte)systemName.Length);
                byte[] systemASCII = Encoding.ASCII.GetBytes(systemName);
                fs.Write(systemASCII);
            }
        }


    }
}
