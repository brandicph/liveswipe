using liveswipe.JPEG.Headers;
using liveswipe.JPEG.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace liveswipe.JPEG.Helpers
{
    public static class BinaryWriterExtension
    {
        public static void WriteHex(this BinaryWriter bwX, int data)
        {
            bwX.Write((byte)(data / 256));
            bwX.Write((byte)(data % 256));
        }

        public static void WriteByteArray(this BinaryWriter bwX, byte[] data, int startPos)
        {
            int len = data.Length;
            for (int i = startPos; i < len; i++)
            {
                bwX.Write((byte)data[i]);
            }
        }

        private static byte bytenew = 0;
        private static sbyte bytepos = 7;
        private static ushort[] mask = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768 };

        public static void WriteBits(this BinaryWriter bw, BitString bs)
        {
            ushort value;
            sbyte posval;

            value = bs.value;
            posval = (sbyte)(bs.length - 1);
            while (posval >= 0)
            {
                if ((value & mask[posval]) != 0)
                {
                    bytenew = (byte)(bytenew | mask[bytepos]);
                }
                posval--;
                bytepos--;
                if (bytepos < 0)
                {
                    // Write to stream
                    if (bytenew == 0xFF)
                    {
                        // Handle special case
                        bw.Write((byte)(0xFF));
                        bw.Write((byte)(0x00));
                    }
                    else bw.Write((byte)(bytenew));
                    // Reinitialize
                    bytepos = 7;
                    bytenew = 0;
                }
            }
        }

        //https://www.w3.org/Graphics/JPEG/jfif3.pdf
        //https://tools.ietf.org/html/rfc2435
        public static void WriteJPEGHeader(this BinaryWriter writer, Xamarin.Forms.Point imageDimensions)
        {
            // JPEG INIT
            writer.WriteAPP0();
            // Write Quantization Header
            writer.WriteDQT();
            // Write Image dimensions
            writer.WriteSOF0((int)imageDimensions.X, (int)imageDimensions.Y);
            // Write Huffman Header
            writer.WriteDHT();
            // Write Color Components
            writer.WriteSOS();

        }
    }
}
