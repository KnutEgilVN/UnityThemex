using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnityThemex
{
    class Program
    {


        static void Main(string[] args)
        {
            string filename = args[0];

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                byte[] temp = new byte[fs.Length];
                fs.Read(temp, 0, temp.Length);

                byte[] themed = ReplaceBytes(temp,
                    new byte[] { 0x84, 0xC0, 0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3 },
                    new byte[] { 0x84, 0xC0, 0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3 });

                fs.Position = 0;
                fs.Write(themed, 0, themed.Length);
            }
        }

        public static int FindBytes(byte[] src, byte[] find)
        {
            int index = -1;
            int matchIndex = 0;
            // handle the complete source array
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == find[matchIndex])
                {
                    if (matchIndex == (find.Length - 1))
                    {
                        index = i - matchIndex;
                        break;
                    }
                    matchIndex++;
                }
                else if (src[i] == find[0])
                {
                    matchIndex = 1;
                }
                else
                {
                    matchIndex = 0;
                }

            }
            return index;
        }
        public static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            byte[] dst = null;
            byte[] temp = null;
            int index = FindBytes(src, search);
            while (index >= 0)
            {
                if (temp == null)
                    temp = src;
                else
                    temp = dst;

                dst = new byte[temp.Length - search.Length + repl.Length];

                // before found array
                Buffer.BlockCopy(temp, 0, dst, 0, index);
                // repl copy
                Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
                // rest of src array
                Buffer.BlockCopy(
                    temp,
                    index + search.Length,
                    dst,
                    index + repl.Length,
                    temp.Length - (index + search.Length));


                index = FindBytes(dst, search);
            }
            return dst;
        }
    }
}
