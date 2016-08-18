using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Security.Permissions;
using liveswipe.JPEG.Models;
using liveswipe.JPEG.Helpers;
using liveswipe.JPEG.Tables;

namespace liveswipe
{    
    public class Encoder
    {
        private sbyte[] Y_Data = new sbyte[64];
        private sbyte[] Cb_Data = new sbyte[64];
        private sbyte[] Cr_Data = new sbyte[64];   
       
        public byte[] LuminanceTable { get; set; } = Luminance.Standard;
        public byte[] ChromianceTable { get; set; } = Chromiance.Standard;

        public int Width = 0;
        public int Height = 0;

        private byte[,,] Bitmap_Buffer = new byte[1,1,1]; 

        private short[] Do_FDCT_Quantization_And_ZigZag(sbyte [] channel_data, float[] quant_table)
        {           

	        float tmp0, tmp1, tmp2, tmp3, tmp4, tmp5, tmp6, tmp7;
	        float tmp10, tmp11, tmp12, tmp13;
	        float z1, z2, z3, z4, z5, z11, z13;	
            float[] temp_data =  new float[64]; 
            short[] outdata = new short[64];
	        float temp;
	        sbyte ctr;
	        byte i;
            int k = 0;

            for (i = 0; i < 64; i++)
            {       
                temp_data[i] = channel_data[i];
            }

	        /* Pass 1: process rows. */	     

	        for (ctr = 7; ctr >= 0; ctr--) 
	        {
                tmp0 = temp_data[0 + k] + temp_data[7 + k];
                tmp7 = temp_data[0 + k] - temp_data[7 + k];
                tmp1 = temp_data[1 + k] + temp_data[6 + k];
                tmp6 = temp_data[1 + k] - temp_data[6 + k];
                tmp2 = temp_data[2 + k] + temp_data[5 + k];
                tmp5 = temp_data[2 + k] - temp_data[5 + k];
                tmp3 = temp_data[3 + k] + temp_data[4 + k];
                tmp4 = temp_data[3 + k] - temp_data[4 + k];

		        /* Even part */

		        tmp10 = tmp0 + tmp3;	/* phase 2 */
		        tmp13 = tmp0 - tmp3;
		        tmp11 = tmp1 + tmp2;
		        tmp12 = tmp1 - tmp2;

                temp_data[0 + k] = tmp10 + tmp11; /* phase 3 */
                temp_data[4 + k] = tmp10 - tmp11;

		        z1 = (tmp12 + tmp13) * ((float) 0.707106781); /* c4 */
                temp_data[2 + k] = tmp13 + z1;	/* phase 5 */
                temp_data[6 + k] = tmp13 - z1;

		        /* Odd part */

		        tmp10 = tmp4 + tmp5;	/* phase 2 */
		        tmp11 = tmp5 + tmp6;
		        tmp12 = tmp6 + tmp7;

		        /* The rotator is modified from fig 4-8 to avoid extra negations. */
		        z5 = (tmp10 - tmp12) * ((float) 0.382683433); /* c6 */
		        z2 = ((float) 0.541196100) * tmp10 + z5; /* c2-c6 */
		        z4 = ((float) 1.306562965) * tmp12 + z5; /* c2+c6 */
		        z3 = tmp11 * ((float) 0.707106781); /* c4 */

		        z11 = tmp7 + z3;		/* phase 5 */
		        z13 = tmp7 - z3;

                temp_data[5 + k] = z13 + z2;	/* phase 6 */
                temp_data[3 + k] = z13 - z2;
                temp_data[1 + k] = z11 + z4;
                temp_data[7 + k] = z11 - z4;	        		

                k += 8;  /* advance pointer to next row */
	        }

          /* Pass 2: process columns. */

            k = 0;
	        
	        for (ctr = 7; ctr >= 0; ctr--) 
	        {
                tmp0 = temp_data[0 + k] + temp_data[56 + k];
                tmp7 = temp_data[0 + k] - temp_data[56 + k];
                tmp1 = temp_data[8 + k] + temp_data[48 + k];
                tmp6 = temp_data[8 + k] - temp_data[48 + k];
                tmp2 = temp_data[16 + k] + temp_data[40 + k];
                tmp5 = temp_data[16 + k] - temp_data[40 + k];
                tmp3 = temp_data[24 + k] + temp_data[32 + k];
                tmp4 = temp_data[24 + k] - temp_data[32 + k];

		        /* Even part */

		        tmp10 = tmp0 + tmp3;	/* phase 2 */
		        tmp13 = tmp0 - tmp3;
		        tmp11 = tmp1 + tmp2;
		        tmp12 = tmp1 - tmp2;

                temp_data[0 + k] = tmp10 + tmp11; /* phase 3 */
                temp_data[32 + k] = tmp10 - tmp11;

		        z1 = (tmp12 + tmp13) * ((float) 0.707106781); /* c4 */
                temp_data[16 + k] = tmp13 + z1; /* phase 5 */
                temp_data[48 + k] = tmp13 - z1;

		        /* Odd part */

		        tmp10 = tmp4 + tmp5;	/* phase 2 */
		        tmp11 = tmp5 + tmp6;
		        tmp12 = tmp6 + tmp7;

		        /* The rotator is modified from fig 4-8 to avoid extra negations. */
		        z5 = (tmp10 - tmp12) * ((float) 0.382683433); /* c6 */
		        z2 = ((float) 0.541196100) * tmp10 + z5; /* c2-c6 */
		        z4 = ((float) 1.306562965) * tmp12 + z5; /* c2+c6 */
		        z3 = tmp11 * ((float) 0.707106781); /* c4 */

		        z11 = tmp7 + z3;		/* phase 5 */
		        z13 = tmp7 - z3;

                temp_data[40 + k] = z13 + z2; /* phase 6 */
                temp_data[24 + k] = z13 - z2;
                temp_data[8  + k] = z11 + z4;
                temp_data[56 + k] = z11 - z4;

		        	
                k++;   /* advance pointer to next column */
	        }

	        // Do Quantization, ZigZag and proper roundoff.
	        for (i = 0; i < 64; i++) 
	        {
                temp = temp_data[i] * quant_table[i];  
		        outdata[ZigZag.Path[i]] = (short) ((short)(temp + 16384.5) - 16384);
	        }            

            return outdata;
        }       

        private void Update_Global_Pixel_8_8_Data(int posX, int posY)
        {             
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte R = Bitmap_Buffer[i + posX, j + posY, 0];
                    byte G = Bitmap_Buffer[i + posX, j + posY, 1];
                    byte B = Bitmap_Buffer[i + posX, j + posY, 2];

                    /*
                    //Compute on the fly
                    Y[i, j] = (byte)(0.299 * R + 0.587 * G + 0.114 * B);
                    Cb[i, j] = (byte)(-0.1687 * R - 0.3313 * G + 0.5 * B + 128);
                    Cr[i, j] = (byte)(0.5 * R - 0.4187 * G - 0.0813 * B + 128);
                    */

                    // Faster to do a table lookup
                    Y_Data[i + j * 8] = (sbyte)(((YCbCr.Y_Red[(R)] + YCbCr.Y_Green[(G)] + YCbCr.Y_Blue[(B)]) >> 16) - 128);
                    Cb_Data[i + j * 8] = (sbyte)((YCbCr.Cb_Red[(R)] + YCbCr.Cb_Green[(G)] + YCbCr.Cb_Blue[(B)]) >> 16);
                    Cr_Data[i + j * 8] = (sbyte)((YCbCr.Cr_Red[(R)] + YCbCr.Cr_Green[(G)] + YCbCr.Cr_Blue[(B)]) >> 16);
                }
            }            
        }       


        /// <summary>
        /// Encodes a provided ImageBuffer[,,] to a JPG Image.
        /// </summary>
        /// <param name="ImageBuffer">The ImageBuffer containing the pixel data.</param>
        /// <param name="originalDimension">Dimension of the original image. This value is written to the image header.</param>
        /// <param name="actualDimension">Dimension on which the Encoder works. As the Encoder works in 8*8 blocks, if the image size is not divisible by 8 the remaining blocks are set to '0' (in this implementation)</param>
        /// <param name="OutputStream">Stream to which the JPEG data is to be written.</param>
        /// <param name="Quantizer_Quality">Required quantizer quality; Default: 50 , Lower value higher quality.</param>
        /// <param name="progress">Interface for updating Progress.</param>
        /// <param name="currentOperation">Interface for updating CurrentOperation.</param>
        public void EncodeImageBufferToJpg(byte[, ,] ImageBuffer, Xamarin.Forms.Point originalDimension, Xamarin.Forms.Point actualDimension, BinaryWriter OutputStream, float Quantizer_Quality, Utils.IProgress progress, Utils.ICurrentOperation currentOperation)
        {
            Width = (int)actualDimension.X;
            Height = (int)actualDimension.Y;

            Bitmap_Buffer = ImageBuffer;

            ushort xpos, ypos;

            currentOperation.SetOperation(Utils.CurrentOperation.InitializingTables);
            Tables.Initialize(Quantizer_Quality, LuminanceTable, ChromianceTable);
            currentOperation.SetOperation(Utils.CurrentOperation.WritingJPEGHeader);
            OutputStream.WriteJPEGHeader(new Xamarin.Forms.Point(originalDimension.X, originalDimension.Y));

            short prev_DC_Y = 0;
            short prev_DC_Cb = 0;
            short prev_DC_Cr = 0;

            currentOperation.SetOperation(Utils.CurrentOperation.EncodeImageBufferToJpg);

            for (ypos = 0; ypos < Height; ypos += 8)
            {
                progress.SetProgress(Height * Width, Width * ypos );
                for (xpos = 0; xpos < Width; xpos += 8)
                {
                    Update_Global_Pixel_8_8_Data(xpos, ypos);
                  
                    // Process Y Channel
                    short[] DCT_Quant_Y = Do_FDCT_Quantization_And_ZigZag(Y_Data, FDCT.Y);                
                    Huffman.Encode(OutputStream, DCT_Quant_Y, ref prev_DC_Y, Huffman.Y_DC, Huffman.Y_AC);

                    // Process Cb Channel
                    short[] DCT_Quant_Cb = Do_FDCT_Quantization_And_ZigZag(Cb_Data, FDCT.CbCr);
                    Huffman.Encode(OutputStream, DCT_Quant_Cb, ref prev_DC_Cb, Huffman.Cb_DC, Huffman.Cb_AC);

                    // Process Cr Channel
                    short[] DCT_Quant_Cr = Do_FDCT_Quantization_And_ZigZag(Cr_Data, FDCT.CbCr);
                    Huffman.Encode(OutputStream, DCT_Quant_Cr, ref prev_DC_Cr, Huffman.Cb_DC, Huffman.Cb_AC);
                }
            }
            OutputStream.WriteHex(0xFFD9); //Write End of Image Marker    

            currentOperation.SetOperation(Utils.CurrentOperation.Ready);
        }

        /// <summary>
        /// Encodes a provided Image to a JPG Image.
        /// </summary>
        /// <param name="ImageToBeEncoded">The Image to be encoded.</param>
        /// <param name="OutputStream">Stream to which the JPEG data is to be written.</param>
        /// <param name="Quantizer_Quality">Required quantizer quality; Default: 50 , Lower value higher quality.</param>
        /// <param name="progress">Interface for updating Progress.</param>
        /// <param name="currentOperation">Interface for updating CurrentOperation.</param>
        public void EncodeImageToJpg(Image ImageToBeEncoded, BinaryWriter OutputStream, float Quantizer_Quality, Utils.IProgress progress,Utils.ICurrentOperation currentOperation)
        {
            Bitmap b_in = new Bitmap(ImageToBeEncoded);
            Width = b_in.Width;
            Height = b_in.Height;
            Xamarin.Forms.Point originalSize =  new Xamarin.Forms.Point(b_in.Width, b_in.Height);
            currentOperation.SetOperation(Utils.CurrentOperation.FillImageBuffer);

            Bitmap_Buffer = Utils.Fill_Image_Buffer(b_in, progress, currentOperation);

            EncodeImageBufferToJpg(Bitmap_Buffer, originalSize, Utils.GetActualDimension(originalSize), OutputStream, 
                Quantizer_Quality,  progress, currentOperation);            
        }
    }
}
