using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using liveswipe.JPEG.Helpers;

namespace liveswipe.JPEG.Headers
{
    public static class SOS
    {
        public static ushort marker = 0xFFDA;
        public static ushort length = 12;
        public static byte nrofcomponents = 3; // Should be 3: truecolor JPG
        public static byte IdY = 1;
        public static byte HTY = 0; // bits 0..3: AC table (0..3)
                                    // bits 4..7: DC table (0..3)
        public static byte IdCb = 2;
        public static byte HTCb = 0x11;
        public static byte IdCr = 3;
        public static byte HTCr = 0x11;
        public static byte Ss = 0, Se = 0x3F, Bf = 0; // not interesting, they should be 0,63,0

        public static void WriteSOS(this BinaryWriter bw)
        {
            bw.WriteHex(marker);
            bw.WriteHex(length);
            bw.Write(nrofcomponents);
            bw.Write(IdY);
            bw.Write(HTY);
            bw.Write(IdCb);
            bw.Write(HTCb);
            bw.Write(IdCr);
            bw.Write(HTCr);
            bw.Write(Ss);
            bw.Write(Se);
            bw.Write(Bf);
        }
    };
}
