using System;
using System.Collections.Generic;
using System.Text;

namespace liveswipe.JPEG.Tables
{
    public static class Quantization
    {
        // Quant data tables
        public static byte[] Y = new byte[64];
        public static byte[] CbCr = new byte[64];

        public static void Compute(float scaleFactor, byte[] luminance_table, byte[] chromiance_table)
        {
            Y = ZigZag.Compute(luminance_table, scaleFactor);
            CbCr = ZigZag.Compute(chromiance_table, scaleFactor);
        }

    }
}
