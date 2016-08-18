using System;
using System.Collections.Generic;
using System.Text;

namespace liveswipe.JPEG.Models
{
    public struct BitString
    {
        public byte length;
        public ushort value;

        public BitString(byte len, ushort val)
        {
            length = len;
            value = val;
        }
    };
}
