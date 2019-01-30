using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    abstract class PlyProperty
    {
        public string Name;

        public PlyProperty(string name)
        {
            this.Name = name;
        }

        public abstract Array CreateArray(int n);

        public abstract object ReadValue(Stream s);

        protected object Parse(byte[] data, int index, PlyPropertyType dataType)
        {
            switch (dataType)
            {
                case PlyPropertyType.Float: return BitConverter.ToSingle(data, index);
                case PlyPropertyType.Int: return BitConverter.ToInt32(data, index);
                case PlyPropertyType.UChar: return data[index];
                default: throw new NotSupportedException("Property type '" + dataType + "' cannot be read");
            }
        }

        protected int ParseCount(byte[] data, int index, PlyPropertyType dataType)
        {
            switch (dataType)
            {
                case PlyPropertyType.Int: return BitConverter.ToInt32(data, index);
                case PlyPropertyType.UChar: return data[index];
                default: throw new NotSupportedException("Property type '" + dataType + "' cannot be interpreted as an integer count of items");
            }
        }

        protected void WriteBytes(object value, PlyPropertyType dataType, Stream s)
        {
            byte[] bytes;
            switch (dataType)
            {
                case PlyPropertyType.Float: bytes = BitConverter.GetBytes((float)value); break;
                case PlyPropertyType.Int: bytes = BitConverter.GetBytes((int)value); break;
                case PlyPropertyType.UChar: bytes = new byte[] { (byte)(Byte)value }; break;
                default: throw new NotSupportedException("Property type '" + dataType + "' cannot be written");
            }
            s.Write(bytes, 0, bytes.Length);
        }

        protected void ReadBytes(Stream s, byte[] data, int index, int nBytes)
        {
            int nRemaining = nBytes;
            int i = index;
            while (true)
            {
                int nRead = s.Read(data, i, nRemaining);
                if (nRead == 0)
                    throw new EndOfStreamException();
                nRemaining -= nRead;
                if (nRemaining == 0)
                    break;
                i += nRead;
            }
        }

        public abstract string HeaderDeclaration { get; }

        public abstract void WriteValue(object value, Stream s);
    }
}
