using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    class PlyPropertySingle : PlyProperty
    {
        public PlyPropertyType DataType;

        private Type _NativeType;
        private int _NativeTypeSize;
        private byte[] _Buffer;

        public PlyPropertySingle(string name, PlyPropertyType dataType)
            : base(name)
        {
            this.Name = name;
            this.DataType = dataType;

            _NativeType = PlyPropertyTypes.TypeOf(this.DataType);
            _NativeTypeSize = System.Runtime.InteropServices.Marshal.SizeOf(_NativeType);

            _Buffer = new byte[_NativeTypeSize];
        }

        public override Array CreateArray(int n)
        {
            return Array.CreateInstance(_NativeType, n);
        }

        public override object ReadValue(Stream s)
        {
            ReadBytes(s, _Buffer, 0, _NativeTypeSize);
            return Parse(_Buffer, 0, this.DataType);
        }

        public override string HeaderDeclaration
        {
            get { return "property " + DataType.ToHeaderString() + ' ' + Name; }
        }

        public override void WriteValue(object value, Stream s)
        {
            WriteBytes(value, DataType, s);
        }
    }
}
