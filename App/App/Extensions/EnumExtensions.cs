using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Extensions
{
    public static class EnumExtensions
    {
        public static T[] FromCommaSeparatedString<T>(this string value)
        {
            return value.Split(new[] { ',' })
                .Select(e => Enum.Parse(typeof(T), e))
                .Cast<T>()
                .ToArray();
        }

        public static string ToCommaSeparatedString<T>(this T[] value) where T : Enum
        {
            return string.Join(",", value.Select(e => e.ToString("D")).ToArray());
        }
    }
}
