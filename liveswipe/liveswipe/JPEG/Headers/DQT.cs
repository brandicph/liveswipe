using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using liveswipe.JPEG.Tables;
using liveswipe.JPEG.Helpers;

namespace liveswipe.JPEG.Headers
{
    public static class DQT
    {
        public static ushort marker = 0xFFDB;
        public static ushort length = 132;  // = 132
        public static byte QTYinfo = 0;// = 0:  bit 0..3: number of QT = 0 (table for Y)
                                       //       bit 4..7: precision of QT, 0 = 8 bit		 
        public static byte QTCbinfo = 1; // = 1 (quantization table for Cb,Cr}             

        public static void WriteDQT(this BinaryWriter bw)
        {
            bw.WriteHex(marker);
            bw.WriteHex(length);
            bw.Write(QTYinfo);
            bw.WriteByteArray(Quantization.Y, 0);
            bw.Write(QTCbinfo);
            bw.WriteByteArray(Quantization.CbCr, 0);
        }
    };
}
