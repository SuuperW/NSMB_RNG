using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace NSMB_RNG
{
    internal unsafe class SeedInitParams
    {
        // Input buttons:
        const uint A = 0x0001;
        const uint B = 0x0002;
        const uint Select = 0x0004;
        const uint Start = 0x0008;
        const uint Right = 0x0010;
        const uint Left = 0x0020;
        const uint Up = 0x0040;
        const uint Down = 0x0080;
        const uint R = 0x0100;
        const uint L = 0x0200;
        const uint X = 0x0400;
        const uint Y = 0x0800;
        const uint Debug = 0x2000;

        private ReadOnlySpan<byte> msgSpan => new ReadOnlySpan<byte>(msg, 32);
        private void* msg;
        const int msgSize = 32;

        public bool is3DS = false;

        public ushort Timer0
        {
            get => ((ushort*)msg)[0];
            set => ((ushort*)msg)[0] = value;
        }
        public ushort VCount
        {
            get => ((ushort*)msg)[1];
            set => ((ushort*)msg)[1] = value;
        }

        private ulong _mac = 0;
        public void SetMAC(ulong value)
        {
            uint vframe = VFrame;
            _mac = value;
            ((uint*)msg)[1] = SwapEndianness((uint)value & 0xffff);
            ((uint*)msg)[2] = SwapEndianness((uint)(_mac >> 16)) ^ 0x06000000 ^ vframe;
        }

        public uint VFrame
        {
            get => ((uint*)msg)[2] ^ 0x06000000 ^ SwapEndianness((uint)(_mac >> 16));
            set => ((uint*)msg)[2] = SwapEndianness((uint)(_mac >> 16)) ^ 0x06000000 ^ value;
        }

        public int Year
        {
            get => FromBCD(((byte*)msg)[12]);
            set => ((byte*)msg)[12] = ToBCD(value);
        }
        public int Month
        {
            get => FromBCD(((byte*)msg)[13]);
            set => ((byte*)msg)[13] = ToBCD(value);
        }
        public int DayOfMonth
        {
            get => FromBCD(((byte*)msg)[14]);
            set => ((byte*)msg)[14] = ToBCD(value);
        }
        public int DayOfWeek
        {
            get => FromBCD(((byte*)msg)[15]);
            set => ((byte*)msg)[15] = ToBCD(value);
        }
        public int Hour
        {
            get
            {
                byte v = ((byte*)msg)[16];
                if (v >= 0x40) v -= 0x40;
                return FromBCD(v);
            }
            set
            {
                byte v = ToBCD(value);
                if (!is3DS && value >= 12) v += 0x40;
                ((byte*)msg)[16] = v;
            } 
        }
        public int Minute
        {
            get => FromBCD(((byte*)msg)[17]);
            set => ((byte*)msg)[17] = ToBCD(value);
        }
        public int Second
        {
            get => FromBCD(((byte*)msg)[18]);
            set => ((byte*)msg)[18] = ToBCD(value);
        }

        public uint Buttons
        {
            get => ((uint*)msg)[7] ^ 0x2FFF;
            set => ((uint*)msg)[7] = value ^ 0x2FFF;
        }

        private byte ToBCD(int input)
        {
            int tens = input / 10;
            return (byte)(tens * 16 + (input % 10));
        }
        private int FromBCD(byte input)
        {
            int tens = input >> 4;
            return tens * 10 + (input & 0xf);
        }

        public SeedInitParams(ulong MACAddress, DateTime dt, bool _3DS = false)
        {
            IntPtr ptr = Marshal.AllocHGlobal(msgSize);
            msg = (void*)ptr;
            long* zeroMe = (long*)ptr;
            for (int i = 0; i < msgSize / sizeof(long); i++)
                zeroMe[i] = 0;

            VFrame = 0; // we have set this before setting the MAC, because of the way the setters handle their overlap
            is3DS = _3DS;
            Buttons = 0;

            SetMAC(MACAddress);
            SetDateTime(dt);
        }

        public SeedInitParams(SeedInitParams other)
        {
            IntPtr ptr = Marshal.AllocHGlobal(msgSize);
            msg = (void*)ptr;
            long* thisMsg = (long*)ptr;
            long* otherMsg = (long*)other.msg;
            for (int i = 0; i < msgSize / sizeof(long); i++)
                thisMsg[i] = otherMsg[i];

            _mac = other._mac;
            is3DS = other.is3DS;
        }

        ~SeedInitParams()
        {
            Marshal.FreeHGlobal((IntPtr)msg);
        }

        private uint SwapEndianness(uint value)
        {
            value = ((value << 8) & 0xFF00FF00) | ((value >> 8) & 0xFF00FF);
            return (value << 16) | (value >> 16);
        }

        public uint GetSeed()
        {
            byte[] sha1 = SHA1.HashData(msgSpan);
            fixed (byte* ptrBytes = sha1)
            {
                uint* sha1Parts = (uint*)ptrBytes;
                return sha1Parts[0] ^ sha1Parts[1] ^ sha1Parts[2] ^ sha1Parts[3] ^ sha1Parts[4];
            }
        }

        public DateTime GetDateTime()
        {
            return new DateTime(Year + 2000, Month, DayOfMonth, Hour, Minute, Second);
        }

        public void SetDateTime(DateTime dt)
        {
            Year = dt.Year - 2000;
            Month = dt.Month;
            DayOfMonth = dt.Day;
            DayOfWeek = (int)dt.DayOfWeek;
            Hour = dt.Hour;
            Minute = dt.Minute;
            Second = dt.Second;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 32; i++)
            {
                sb.Append(msgSpan[i].ToString("X")).Append(" ");
            }
            return sb.ToString();
        }

    }
}
