using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace App.Extensions
{
    public static class ColorExtensions
    {
        public static Color ToColor(this int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public static int ToInt32(this Color value)
        {
            byte[] bytes = new byte[4];
            bytes[2] = value.R;
            bytes[1] = value.G;
            bytes[0] = value.B;
            bytes[3] = value.A;
            return BitConverter.ToInt32(bytes);
        }
    }
}
