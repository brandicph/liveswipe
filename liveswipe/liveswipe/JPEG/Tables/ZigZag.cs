using System;
using System.Collections.Generic;
using System.Text;

namespace liveswipe.JPEG.Tables
{
    public static class ZigZag
    {
        public static byte[] Path =
        {
            0, 1, 5, 6,14,15,27,28,
            2, 4, 7,13,16,26,29,42,
            3, 8,12,17,25,30,41,43,
            9,11,18,24,31,40,44,53,
            10,19,23,32,39,45,52,54,
            20,22,33,38,46,51,55,60,
            21,34,37,47,50,56,59,61,
            35,36,48,49,57,58,62,63
        };

        public static byte[] Compute(byte[] inTable, float quality_scale)
        {
            byte[] outTable = new byte[64];
            long temp;
            for (byte i = 0; i < 64; i++)
            {
                temp = ((long)(inTable[i] * quality_scale + 50L) / 100L);
                if (temp <= 0L)
                    temp = 1L;
                if (temp > 255L)
                    temp = 255L;
                outTable[Path[i]] = (byte)temp;
            }
            return outTable;
        }
    }
}
