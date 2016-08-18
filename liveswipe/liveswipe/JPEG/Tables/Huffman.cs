using liveswipe.JPEG.Helpers;
using liveswipe.JPEG.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace liveswipe.JPEG.Tables
{
    public static class Huffman
    {
        public static BitString[] Y_DC = new BitString[12];
        public static BitString[] Cb_DC = new BitString[12];
        public static BitString[] Y_AC = new BitString[256];
        public static BitString[] Cb_AC = new BitString[256];

        public static void Compute(byte[] nrCodes, byte[] std_table, ref BitString[] table)
        {
            byte k, j;
            byte pos_in_table;
            ushort code_value;

            code_value = 0;
            pos_in_table = 0;
            for (k = 1; k <= 16; k++)
            {
                for (j = 1; j <= nrCodes[k]; j++)
                {
                    table[std_table[pos_in_table]].value = code_value;
                    table[std_table[pos_in_table]].length = k;
                    pos_in_table++;
                    code_value++;
                }
                code_value <<= 1;
            }
        }

        public static void Initialize()
        {
            Huffman.Compute(Luminance.Standard_DC_NRCodes, Luminance.Standard_DC_Values, ref Huffman.Y_DC);
            Huffman.Compute(Luminance.Standard_AC_NRCodes, Luminance.Standard_AC_Values, ref Huffman.Y_AC);
            Huffman.Compute(Chromiance.Standard_DC_NRCodes, Chromiance.Standard_DC_Values, ref Huffman.Cb_DC);
            Huffman.Compute(Chromiance.Standard_AC_NRCodes, Chromiance.Standard_AC_Values, ref Huffman.Cb_AC);
        }

        public static void Encode(this BinaryWriter bw, short[] DU, ref short DC, BitString[] HTDC, BitString[] HTAC)
        {
            BitString EOB = HTAC[0x00];
            BitString M16zeroes = HTAC[0xF0];
            byte i;
            byte startpos;
            byte end0pos;
            byte nrzeroes;
            byte nrmarker;
            short Diff;

            // Encode DC
            Diff = (short)(DU[0] - DC);
            DC = DU[0];

            if (Diff == 0)
                bw.WriteBits(HTDC[0]);
            else
            {
                bw.WriteBits(HTDC[Prediction.Category[32767 + Diff]]);
                bw.WriteBits(Prediction.Bitcode[32767 + Diff]);
            }

            // Encode ACs
            for (end0pos = 63; (end0pos > 0) && (DU[end0pos] == 0); end0pos--) ;
            //end0pos = first element in reverse order != 0

            i = 1;
            while (i <= end0pos)
            {
                startpos = i;
                for (; (DU[i] == 0) && (i <= end0pos); i++) ;
                nrzeroes = (byte)(i - startpos);
                if (nrzeroes >= 16)
                {
                    for (nrmarker = 1; nrmarker <= nrzeroes / 16; nrmarker++)
                        bw.WriteBits(M16zeroes);
                    nrzeroes = (byte)(nrzeroes % 16);
                }
                bw.WriteBits(HTAC[nrzeroes * 16 + Prediction.Category[32767 + DU[i]]]);
                bw.WriteBits(Prediction.Bitcode[32767 + DU[i]]);
                i++;
            }

            if (end0pos != 63)
                bw.WriteBits(EOB);
        }
    }
}
