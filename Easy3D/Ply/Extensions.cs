using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
                action(item);
        }

        public static Color ToGlColor(this uint c)
        {
            return Color.FromArgb((int)(c >> 24), (int)(c & 0xFF), (int)((c >> 8) & 0xFF), (int)((c >> 16) & 0xFF));
        }

        public static uint ToGlColor(this Color c)
        {
            return ((uint)c.A << 24) | ((uint)c.B << 16) | ((uint)c.G << 8) | (uint)c.R;
        }
    }
}
