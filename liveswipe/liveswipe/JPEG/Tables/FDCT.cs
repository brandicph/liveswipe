using System;
using System.Collections.Generic;
using System.Text;

namespace liveswipe.JPEG.Tables
{
    public static class FDCT
    {
        // Quant data tables after scaling and cos.
        public static float[] Y = new float[64];
        public static float[] CbCr = new float[64];


        public static void Initialize()
        {
            double[] CosineScaleFactor = { 1.0, 1.387039845, 1.306562965, 1.175875602, 1.0, 0.785694958, 0.541196100, 0.275899379 };

            byte i = 0;

            for (byte row = 0; row < 8; row++)
            {
                for (byte col = 0; col < 8; col++)
                {
                    Y[i] = (float)(1.0 / ((double)Y[ZigZag.Path[i]] *
                        CosineScaleFactor[row] * CosineScaleFactor[col] * 8.0));
                    CbCr[i] = (float)(1.0 / ((double)CbCr[ZigZag.Path[i]] *
                        CosineScaleFactor[row] * CosineScaleFactor[col] * 8.0));
                    i++;
                }
            }
        }
    }
}
