using liveswipe.JPEG.Helpers;
using liveswipe.JPEG.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace liveswipe.JPEG.Headers
{
    public static class DHT
    {

        public static ushort marker = 0xFFC4;
        public static ushort length = 0x01A2;
        public static byte HTYDC = 0x00; // bit 0..3: number of HT (0..3), for Y =0
                                         //bit 4  :type of HT, 0 = DC table,1 = AC table
                                         //bit 5..7: not used, must be 0
        public static byte HTYAC = 0x10; // = 0x10
        public static byte HTCbDC = 0x01; // = 1
        public static byte HTCbAC = 0x11; //  = 0x11

        public static void WriteDHT(this BinaryWriter bw)
        {
            bw.WriteHex(marker);
            bw.WriteHex(length);
            bw.Write(HTYDC);
            bw.WriteByteArray(Luminance.Standard_DC_NRCodes, 1); //at index i = nr of codes with length i
            bw.WriteByteArray(Luminance.Standard_DC_Values, 0);
            bw.Write(HTYAC);
            bw.WriteByteArray(Luminance.Standard_AC_NRCodes, 1);
            bw.WriteByteArray(Luminance.Standard_AC_Values, 0); //we'll use the standard Huffman tables
            bw.Write(HTCbDC);
            bw.WriteByteArray(Chromiance.Standard_DC_NRCodes, 1);
            bw.WriteByteArray(Chromiance.Standard_DC_Values, 0);
            bw.Write(HTCbAC);
            bw.WriteByteArray(Chromiance.Standard_AC_NRCodes, 1);
            bw.WriteByteArray(Chromiance.Standard_AC_Values, 0);
        }
    };
}
