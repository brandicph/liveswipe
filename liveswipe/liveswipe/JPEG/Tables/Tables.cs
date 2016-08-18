namespace liveswipe.JPEG.Tables
{
    public static class Tables
    {
        public static void Initialize(float quantizer_quality, byte[] luminance_table, byte[] chromiance_table)
        {
            // Precalculate the RGB > YCbCr tables 
            YCbCr.Initialize();

            // Compute Quantization tables based on quality and luminance/chromiance tables
            Quantization.Compute((float)quantizer_quality, luminance_table, chromiance_table);

            // Initialize the Huffman tables used for encoding
            Huffman.Initialize();

            // Initialize Category and Bitcode
            Prediction.Initialize();

            // Initialize FDCT tables
            FDCT.Initialize();
        }

    }
}
