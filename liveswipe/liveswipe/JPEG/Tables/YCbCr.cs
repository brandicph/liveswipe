using System;
using System.Collections.Generic;
using System.Text;

namespace liveswipe.JPEG.Tables
{
    public static class YCbCr
    {
        // Y, Cb, Cr Tables
        public static int[] Y_Red = new int[256];
        public static int[] Y_Green = new int[256];
        public static int[] Y_Blue = new int[256];
        public static int[] Cb_Red = new int[256];
        public static int[] Cb_Green = new int[256];
        public static int[] Cb_Blue = new int[256];
        public static int[] Cr_Red = new int[256];
        public static int[] Cr_Green = new int[256];
        public static int[] Cr_Blue = new int[256];

        public static void Initialize()
        {
            ushort R, G, B;
            for (R = 0; R < 256; R++)
            {
                B = G = R;
                Y_Red[R] = (int)((65536 * 0.299 + 0.5) * R);
                Cb_Red[R] = (int)((65536 * -0.16874 + 0.5) * R);
                Cr_Red[R] = (int)((32768) * R);

                Y_Green[G] = (int)((65536 * 0.587 + 0.5) * G);
                Cb_Green[G] = (int)((65536 * -0.33126 + 0.5) * G);
                Cr_Green[G] = (int)((65536 * -0.41869 + 0.5) * G);

                Y_Blue[B] = (int)((65536 * 0.114 + 0.5) * B);
                Cb_Blue[B] = (int)((32768) * B);
                Cr_Blue[B] = (int)((65536 * -0.08131 + 0.5) * B);
            }
        }

    }
}
