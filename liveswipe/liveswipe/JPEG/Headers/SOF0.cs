using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using liveswipe.JPEG.Helpers;

namespace liveswipe.JPEG.Headers
{
    public static class SOF0
    {
        public static ushort marker = 0xFFC0;
        public static ushort length = 17; // = 17 for a truecolor YCbCr JPG
        public static byte precision = 8;// Should be 8: 8 bits/sample            
        public static byte nrofcomponents = 3;//Should be 3: We encode a truecolor JPG
        public static byte IdY = 1;  // = 1
        public static byte HVY = 0x11; // sampling factors for Y (bit 0-3 vert., 4-7 hor.)
        public static byte QTY = 0;  // Quantization Table number for Y = 0
        public static byte IdCb = 2; // = 2
        public static byte HVCb = 0x11;
        public static byte QTCb = 1; // 1
        public static byte IdCr = 3; // = 3
        public static byte HVCr = 0x11;
        public static byte QTCr = 1; // Normally equal to QTCb = 1

        public static void WriteSOF0(this BinaryWriter bw, int wid, int ht)
        {
            bw.WriteHex(marker);
            bw.WriteHex(length);
            bw.Write(precision);
            bw.WriteHex(ht);
            bw.WriteHex(wid);
            bw.Write(nrofcomponents);
            bw.Write(IdY);
            bw.Write(HVY);
            bw.Write(QTY);
            bw.Write(IdCb);
            bw.Write(HVCb);
            bw.Write(QTCb);
            bw.Write(IdCr);
            bw.Write(HVCr);
            bw.Write(QTCr);

        }
    };
}
