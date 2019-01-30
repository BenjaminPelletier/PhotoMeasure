using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    public enum PlyFormat
    {
        Ascii10,
        BinaryLittleEndian10,
        BinaryBigEndian10,
    }

    static class PlyFormatConversions
    {
        private static Dictionary<string, PlyFormat> _StringToFormat = new Dictionary<string, PlyFormat>()
        {
            {"format ascii 1.0", PlyFormat.Ascii10 },
            {"format binary_little_endian 1.0", PlyFormat.BinaryLittleEndian10 },
            {"format binary_big_endian 1.0", PlyFormat.BinaryBigEndian10 }
        };

        private static Dictionary<PlyFormat, string> _FormatToString = _StringToFormat.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static PlyFormat FromString(string s)
        {
            if (_StringToFormat.ContainsKey(s))
                return _StringToFormat[s];
            else
                throw new NotSupportedException("This importer does not support the format '" + s.Substring("format ".Length) + "'");
        }

        public static string ToHeaderString(this PlyFormat f)
        {
            return _FormatToString[f];
        }
    }
}
