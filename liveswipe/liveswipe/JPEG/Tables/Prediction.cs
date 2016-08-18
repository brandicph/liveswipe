using liveswipe.JPEG.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace liveswipe.JPEG.Tables
{
    public static class Prediction
    {
        public static byte[] Category = new byte[65535];
        public static BitString[] Bitcode = new BitString[65535];

        //https://books.google.dk/books?id=C-bgBwAAQBAJ&pg=PA48&lpg=PA48&dq=jpeg+Category+BitCode&source=bl&ots=HpUICYOyAN&sig=TEbIKAT4xsa_iUcu59W_dJRLcHg&hl=en&sa=X&ved=0ahUKEwii06PBjcPOAhXD3CwKHUUGBmkQ6AEIKzAD#v=onepage&q=jpeg%20Category%20BitCode&f=false
        //https://www.google.com/patents/USRE39925
        public static void Initialize()
        {
            int nr;
            int nr_lower, nr_upper;
            byte cat;

            nr_lower = 1;
            nr_upper = 2;
            for (cat = 1; cat <= 15; cat++)
            {
                //Positive numbers
                for (nr = nr_lower; nr < nr_upper; nr++)
                {
                    Category[32767 + nr] = cat;
                    Bitcode[32767 + nr].length = cat;
                    Bitcode[32767 + nr].value = (ushort)nr;
                }
                //Negative numbers
                for (nr = -(nr_upper - 1); nr <= -nr_lower; nr++)
                {
                    Category[32767 + nr] = cat;
                    Bitcode[32767 + nr].length = cat;
                    Bitcode[32767 + nr].value = (ushort)(nr_upper - 1 + nr);
                }
                nr_lower <<= 1;
                nr_upper <<= 1;
            }
        }
    }
}
