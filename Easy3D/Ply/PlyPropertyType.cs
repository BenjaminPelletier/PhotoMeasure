using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    public enum PlyPropertyType
    {
        Float,
        UChar,
        Int
    }

    public static class PlyPropertyTypes
    {
        private static Dictionary<string, PlyPropertyType> _StringToType = new Dictionary<string, PlyPropertyType>()
        {
            {"float", PlyPropertyType.Float},
            {"uchar", PlyPropertyType.UChar},
            {"int", PlyPropertyType.Int},
        };

        private static Dictionary<PlyPropertyType, string> _TypeToString = _StringToType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        private static Dictionary<PlyPropertyType, Type> _NativeTypes = new Dictionary<PlyPropertyType, Type>()
        {
            {PlyPropertyType.Float, typeof(float)},
            {PlyPropertyType.Int, typeof(int)},
            {PlyPropertyType.UChar, typeof(byte)},
        };

        public static bool IsValid(string s)
        {
            return _StringToType.ContainsKey(s);
        }

        public static PlyPropertyType Parse(string s)
        {
            return _StringToType[s];
        }

        public static Type TypeOf(PlyPropertyType t)
        {
            return _NativeTypes[t];
        }

        public static string ToHeaderString(this PlyPropertyType t)
        {
            return _TypeToString[t];
        }
    }
}
