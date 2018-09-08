﻿using System.IO;

namespace Core.Utils.CRC
{
    class SlicingBy8
    {
        const int bufSize = 16384;
        uint[] u32Table0, u32Table1, u32Table2, u32Table3, u32Table4, u32Table5, u32Table6, u32Table7;
        uint[] uTable;
        uint Crc32;
        byte[] u8Buf;
        public SlicingBy8()
        {
            u8Buf = new byte[bufSize];
            InitCrc();
            unchecked
            {
                uint dwPolynomial = 0xEDB88320;
                //                uint dwPolynomial = 0x82F63B78;
                uint i, j;

                u32Table0 = new uint[256];
                u32Table1 = new uint[256];
                u32Table2 = new uint[256];
                u32Table3 = new uint[256];
                u32Table4 = new uint[256];
                u32Table5 = new uint[256];
                u32Table6 = new uint[256];
                u32Table7 = new uint[256];
                uTable = new uint[32 * 256];

                uint u32Crc;
                for (i = 0; i < 256; i++)
                {
                    u32Crc = i;
                    for (j = 8; j > 0; j--)
                    {
                        if ((u32Crc & 1) == 1)
                        {
                            u32Crc = (u32Crc >> 1) ^ dwPolynomial;
                        }
                        else
                        {
                            u32Crc >>= 1;
                        }
                    }
                    u32Table0[i] = u32Crc;
                    uTable[i] = u32Crc;
                }
                //Slicing by 8 Table
                for (i = 0; i < 256; i++)
                {
                    u32Crc = u32Table0[i];
                    u32Crc = u32Table1[i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = u32Table2[i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = u32Table3[i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = u32Table4[i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = u32Table5[i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = u32Table6[i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Table7[i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                }
                //Slicing by 32 用テーブル。8にも16にも流用可能
                for (i = 0; i < 256; i++)
                {
                    u32Crc = uTable[i];
                    u32Crc = uTable[256 * 1 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 2 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 3 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 4 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 5 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 6 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 7 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 8 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 9 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 10 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 11 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 12 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 13 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 14 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 15 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 16 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 17 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 18 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 19 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 20 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 21 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 22 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 23 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 24 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 25 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 26 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 27 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 28 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 29 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    u32Crc = uTable[256 * 30 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                    uTable[256 * 31 + i] = u32Table0[u32Crc & 0xff] ^ (u32Crc >> 8);
                }
            }
        }
        public void WarmTable()
        {
            int i;
            uint j;
            for (i = 0; i < 256; i++)
            {
                j = u32Table0[i];
                j = u32Table1[i];
                j = u32Table2[i];
                j = u32Table3[i];
                j = u32Table4[i];
                j = u32Table5[i];
                j = u32Table6[i];
                j = u32Table7[i];
            }
        }

        public void InitCrc()
        {
            Crc32 = 0xFFFFFFFF;
        }
        public uint DoSb8(Stream s)
        {
            int count, i, blocks, rem;
            s.Seek(0L, SeekOrigin.Begin);
            while ((count = s.Read(u8Buf, 0, bufSize)) > 0)
            {
                blocks = (count / 8) * 8;
                for (i = 0; i < blocks; i = i + 8)
                {
                    Crc32 ^= (uint)(u8Buf[i] | u8Buf[i + 1] << 8 | u8Buf[i + 2] << 16 | u8Buf[i + 3] << 24);
                    Crc32 = u32Table7[Crc32 & 0xFF] ^ u32Table6[(Crc32 >> 8) & 0xFF] ^
                            u32Table5[(Crc32 >> 16) & 0xFF] ^ u32Table4[(Crc32 >> 24) & 0xFF] ^
                            u32Table3[u8Buf[i + 4]] ^ u32Table2[u8Buf[i + 5]] ^
                            u32Table1[u8Buf[i + 6]] ^ u32Table0[u8Buf[i + 7]];
                }
            }
            //端数処理。本当はもっとましなやり方をすべきだけどメインループの外に出さないと遅すぎる
            rem = (int)s.Length % 8;
            s.Position = s.Length - rem;
            s.Read(u8Buf, 0, rem);
            for (i = 0; i < rem; i++)
            {
                Crc32 = (Crc32 >> 8) ^ u32Table0[(Crc32 ^ u8Buf[i]) & 0xFF];
            }
            return Crc32 ^ 0xFFFFFFFF;
        }

        public uint UnsafeDoSb8(Stream s)
        {
            int count, i, blocks, first, last;
            s.Seek(0L, SeekOrigin.Begin);
            unsafe
            {
                while ((count = s.Read(u8Buf, 0, bufSize)) > 0)
                {
                    fixed (byte* u8pc = u8Buf)
                    {
                        byte* u8p = u8pc; ;
                        last = 0;
                        //uint ubuf;
                        first = ((int)u8p) % 4;
                        if (first > count)
                        {
                            first = count;
                        }
                        blocks = ((count - first) / 8);
                        last = count - (blocks * 8) - first;
                        //Console.WriteLine("hassu:" + first);

                        for (i = 0; i < first; i++)
                        {
                            Crc32 = (Crc32 >> 8) ^ u32Table0[(Crc32 ^ *u8p) & 0xFF];
                            u8p++;
                        }
                        for (i = 0; i < blocks; i++)
                        {
                            Crc32 ^= *(uint*)u8p;
                            u8p += 4;
                            //ubuf = *(uint*)u8p;

                            Crc32 = u32Table7[Crc32 & 0xFF] ^ u32Table6[(Crc32 >> 8) & 0xFF] ^
                                    u32Table5[(Crc32 >> 16) & 0xFF] ^ u32Table4[(Crc32 >> 24) & 0xFF] ^
                                    u32Table3[*(uint*)u8p & 0xFF] ^ u32Table2[(*(uint*)u8p >> 8) & 0xFF] ^
                                    u32Table1[(*(uint*)u8p >> 16) & 0xFF] ^ u32Table0[(*(uint*)u8p >> 24) & 0xFF];
                            u8p += 4;
                        }
                        //u8p = (byte*)u32p;
                        for (i = 0; i < last; i++)
                        {
                            Crc32 = (Crc32 >> 8) ^ u32Table0[(Crc32 ^ *u8p) & 0xFF];
                            u8p++;
                        }
                    }
                }
            }
            return Crc32 ^ 0xFFFFFFFF;
        }
        public uint DoSarwate(Stream s)
        {
            int count, i;
            s.Seek(0L, SeekOrigin.Begin);
            count = s.Read(u8Buf, 0, bufSize);
            while (count > 0)
            {
                for (i = 0; i < count; i++)
                {
                    Crc32 = (Crc32 >> 8) ^ u32Table0[(Crc32 ^ u8Buf[i]) & 0xFF];
                }
                count = s.Read(u8Buf, 0, bufSize);
            }
            return 0xFFFFFFFF ^ Crc32;
        }

        /// <summary>
        /// Slicing by 32
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks>Writed by IL</remarks>
        public uint DoSb32(Stream s)
        {
            int count, i, blocks, rem;
            s.Seek(0L, SeekOrigin.Begin);
            while ((count = s.Read(u8Buf, 0, bufSize)) > 0)
            {
                blocks = (count / 32) * 32;
                for (i = 0; i < blocks; i = i + 32)
                {
                    Crc32 ^= (uint)(u8Buf[i] | u8Buf[i + 1] << 8 | u8Buf[i + 2] << 16 | u8Buf[i + 3] << 24);

                    Crc32 = uTable[(256 * 31) + (Crc32 & 0xFF)] ^ uTable[(256 * 30) + ((Crc32 >> 8) & 0xFF)] ^
                            uTable[(256 * 29) + ((Crc32 >> 16) & 0xFF)] ^ uTable[(256 * 28) + ((Crc32 >> 24) & 0xFF)] ^
                            uTable[(256 * 27) + (u8Buf[i + 4])] ^ uTable[(256 * 26) + (u8Buf[i + 5])] ^
                            uTable[(256 * 25) + (u8Buf[i + 6])] ^ uTable[(256 * 24) + (u8Buf[i + 7])] ^
                            uTable[(256 * 23) + (u8Buf[i + 8])] ^ uTable[(256 * 22) + (u8Buf[i + 9])] ^
                            uTable[(256 * 21) + (u8Buf[i + 10])] ^ uTable[(256 * 20) + (u8Buf[i + 11])] ^
                            uTable[(256 * 19) + (u8Buf[i + 12])] ^ uTable[(256 * 18) + (u8Buf[i + 13])] ^
                            uTable[(256 * 17) + (u8Buf[i + 14])] ^ uTable[(256 * 16) + (u8Buf[i + 15])] ^
                            uTable[(256 * 15) + (u8Buf[i + 16])] ^ uTable[(256 * 14) + (u8Buf[i + 17])] ^
                            uTable[(256 * 13) + (u8Buf[i + 18])] ^ uTable[(256 * 12) + (u8Buf[i + 19])] ^
                            uTable[(256 * 11) + (u8Buf[i + 20])] ^ uTable[(256 * 10) + (u8Buf[i + 21])] ^
                            uTable[(256 * 9) + (u8Buf[i + 22])] ^ uTable[(256 * 8) + (u8Buf[i + 23])] ^
                            uTable[(256 * 7) + (u8Buf[i + 24])] ^ uTable[(256 * 6) + (u8Buf[i + 25])] ^
                            uTable[(256 * 5) + (u8Buf[i + 26])] ^ uTable[(256 * 4) + (u8Buf[i + 27])] ^
                            uTable[(256 * 3) + (u8Buf[i + 28])] ^ uTable[(256 * 2) + (u8Buf[i + 29])] ^
                            uTable[(256 * 1) + (u8Buf[i + 30])] ^ uTable[(256 * 0) + (u8Buf[i + 31])];
                }
            }
            //端数処理。本当はもっとましなやり方をすべきだけどメインループの外に出さないと遅すぎる
            rem = (int)s.Length % 32;
            s.Position = s.Length - rem;
            s.Read(u8Buf, 0, rem);
            for (i = 0; i < rem; i++)
            {
                Crc32 = (Crc32 >> 8) ^ uTable[(Crc32 ^ u8Buf[i]) & 0xFF];
            }
            return Crc32 ^ 0xFFFFFFFF;
        }
    }
}