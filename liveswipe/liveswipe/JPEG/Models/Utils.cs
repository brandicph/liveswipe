
using System;
using System.IO;
using System.Drawing;

namespace liveswipe
{
    public  class Utils
    {
        public interface IProgress
        {
            void SetProgress(int FullProgress, int CurrentProgress);
        };

        public class ProgressUpdater : IProgress
        {
            public int Full, Current;

            void Utils.IProgress.SetProgress(int FullProgress, int CurrentProgress)
            {
                Full = FullProgress;
                Current = CurrentProgress;
            }
        }

        public enum CurrentOperation { PrecalculateYCbCrTables, InitializingTables, WritingJPEGHeader , FillImageBuffer, EncodeImageBufferToJpg, GetChannelData ,WriteChannelImages, Ready};
        
        public interface ICurrentOperation
        {
            void SetOperation(CurrentOperation currentOperation);
        };

        public class CurrentOperationUpdater : ICurrentOperation
        {
            public CurrentOperation operation;

            void ICurrentOperation.SetOperation(CurrentOperation currentOperation)
            {
                operation = currentOperation;                
            }
        }     

        public static byte[,,] Fill_Image_Buffer(Bitmap bmp, IProgress progress,ICurrentOperation operation)
        {
            operation.SetOperation(CurrentOperation.FillImageBuffer);
            Xamarin.Forms.Point originalSize = GetActualDimension(new Xamarin.Forms.Point(bmp.Width, bmp.Height));
            byte[,,] Bitmap_Buffer = new byte[(int)originalSize.X,(int)originalSize.Y, 3];
            
            IntPtr hbmScreen = IntPtr.Zero;
            IntPtr hBmpInput = bmp.GetHbitmap();

            InteropGDI.BITMAP bmpInput = new InteropGDI.BITMAP();
            InteropGDI.GetObject(hBmpInput, 24, ref bmpInput);  
            InteropGDI.BITMAPINFOHEADER bi;

            bi.biSize = 40;
            bi.biWidth = bmpInput.bmWidth;
            bi.biHeight = -bmpInput.bmHeight; // Negative to reverse the scan order.
            bi.biPlanes = 1;
            bi.biBitCount = 32;
            bi.biCompression = (uint)InteropGDI.BMP_Compression_Modes.BI_RGB; 
            bi.biSizeImage = 0;
            bi.biXPelsPerMeter = 0;
            bi.biYPelsPerMeter = 0;
            bi.biClrUsed = 0;
            bi.biClrImportant = 0;

            ulong bitmapLengthbytes = (ulong)(((bmpInput.bmWidth * bi.biBitCount + 31) / 32) * 4 * bmpInput.bmHeight);

            byte[] bitmap_array = new byte[bitmapLengthbytes];

            IntPtr hdcWindow = InteropGDI.CreateCompatibleDC(IntPtr.Zero);

            InteropGDI.GetDIBits(hdcWindow, hBmpInput, (uint)0,
                (uint)bmpInput.bmHeight,
                bitmap_array,
                ref bi, (uint)InteropGDI.DIB_COLORS.DIB_RGB_COLORS);

            int k = 0, wd = bmpInput.bmWidth, ht = bmpInput.bmHeight;           

            for (int j = 0; j < ht; j++)
            {
                progress.SetProgress((int)bitmapLengthbytes, k);
                for (int i = 0; i < wd; i++)
                {
                    Bitmap_Buffer[i, j, 2] = bitmap_array[k++];
                    Bitmap_Buffer[i, j, 1] = bitmap_array[k++];
                    Bitmap_Buffer[i, j, 0] = bitmap_array[k++];
                    k++;                    
                }
            }

            InteropGDI.DeleteObject(hdcWindow);
            InteropGDI.DeleteObject(hbmScreen);
            InteropGDI.DeleteObject(hBmpInput);           

            return Bitmap_Buffer;
        }

        public static Bitmap Write_Bmp_From_Data(byte [,,] pixel_data, Xamarin.Forms.Point dimensions, IProgress progress, ICurrentOperation operation)
        {
            operation.SetOperation(Utils.CurrentOperation.WriteChannelImages); 

            int k = 0;
            Bitmap bmp = new Bitmap((int)dimensions.X,(int)dimensions.Y);
            IntPtr hBmpOutput = bmp.GetHbitmap(); 
            byte[, ,] Bitmap_Buffer = new byte[(int)dimensions.X, (int)dimensions.Y, 3]; 

            InteropGDI.BITMAPINFOHEADER bi;

            bi.biSize = 40;
            bi.biWidth = (int)dimensions.X;
            bi.biHeight = (int)-dimensions.Y;
            bi.biPlanes = 1;
            bi.biBitCount = 32;
            bi.biCompression = (uint)InteropGDI.BMP_Compression_Modes.BI_RGB; 
            bi.biSizeImage = 0;
            bi.biXPelsPerMeter = 0;
            bi.biYPelsPerMeter = 0;
            bi.biClrUsed = 0;
            bi.biClrImportant = 0;            

            ulong bitmapLengthbytes = (ulong)(((dimensions.X * bi.biBitCount + 31) / 32) * 4 * dimensions.Y);
            
            byte[] bitmap_array = new byte[bitmapLengthbytes];

            for (int j = 0; j < dimensions.Y; j++)
            {
                progress.SetProgress((int)bitmapLengthbytes, k);
                for (int i = 0; i < dimensions.X; i++)
                {
                    bitmap_array[k++] = pixel_data[i, j, 2];
                    bitmap_array[k++] = pixel_data[i, j, 1];
                    bitmap_array[k++] = pixel_data[i, j, 0];
                    k++;                    
                }
            }             

            IntPtr hdcWindow = InteropGDI.CreateCompatibleDC(IntPtr.Zero);

            InteropGDI.SetDIBits(hdcWindow, hBmpOutput, (uint)0,
                (uint)dimensions.Y,
                bitmap_array,
                ref bi, (uint)InteropGDI.DIB_COLORS.DIB_RGB_COLORS);

            bmp = Bitmap.FromHbitmap(hBmpOutput);

            InteropGDI.DeleteObject(hBmpOutput);          
            InteropGDI.DeleteObject(hdcWindow);

            return bmp;
        }             

        public static Xamarin.Forms.Point GetActualDimension(Xamarin.Forms.Point originalDim)
        {
            int width_8, height_8;

            if (originalDim.X % 8 != 0)
                width_8 = (int)(originalDim.X / 8) * 8 + 8;
            else
                width_8 = (int)originalDim.X;

            if (originalDim.Y % 8 != 0)
                height_8 = (int)(originalDim.Y / 8) * 8 + 8;
            else
                height_8 = (int)originalDim.Y;   

            return new Xamarin.Forms.Point(width_8, height_8);            
        }

    }
}
