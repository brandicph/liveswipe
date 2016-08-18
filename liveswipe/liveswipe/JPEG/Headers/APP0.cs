using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using liveswipe.JPEG.Helpers;

namespace liveswipe.JPEG.Headers
{
    public static class APP0
    {
        public static ushort marker = 0xFFE0;
        public static ushort length = 16; // = 16 for usual JPEG, no thumbnail		
        public static byte versionhi = 1; // 1
        public static byte versionlo = 1; // 1
        public static byte xyunits = 0;   // 0 = no units, normal density
        public static ushort xdensity = 1;  // 1
        public static ushort ydensity = 1;  // 1
        public static byte thumbnwidth = 0; // 0
        public static byte thumbnheight = 0; // 0

        public static void WriteAPP0(this BinaryWriter bw)
        {
            bw.WriteHex(0xFFD8); // JPEG INIT
            bw.WriteHex(marker);
            bw.WriteHex(length);
            bw.Write('J');
            bw.Write('F');
            bw.Write('I');
            bw.Write('F');
            bw.Write((byte)0x0);
            bw.Write(versionhi);
            bw.Write(versionlo);
            bw.Write(xyunits);
            bw.WriteHex(xdensity);
            bw.WriteHex(ydensity);
            bw.Write(thumbnheight);
            bw.Write(thumbnwidth);

        }
    };
}
