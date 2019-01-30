using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    class PlyPropertyList : PlyProperty
    {
        const int MAX_LIST_ITEMS = 100;

        public PlyPropertyType DataType;

        private PlyPropertyType _CountType;
        private int _CountSize;

        private Type _NativeType;
        private int _NativeTypeSize;

        private byte[] _Buffer;

        public PlyPropertyList(string name, PlyPropertyType dataType, PlyPropertyType countType)
            : base(name)
        {
            this.Name = name;
            this.DataType = dataType;

            _NativeType = PlyPropertyTypes.TypeOf(this.DataType);
            _NativeTypeSize = System.Runtime.InteropServices.Marshal.SizeOf(_NativeType);

            _CountType = countType;
            _CountSize = System.Runtime.InteropServices.Marshal.SizeOf(PlyPropertyTypes.TypeOf(countType));

            _Buffer = new byte[_NativeTypeSize * MAX_LIST_ITEMS];
        }

        public override Array CreateArray(int n)
        {
            return Array.CreateInstance(_NativeType.MakeArrayType(), n);
        }

        public override object ReadValue(Stream s)
        {
            ReadBytes(s, _Buffer, 0, _CountSize);
            int n = ParseCount(_Buffer, 0, _CountType);
            if (n > MAX_LIST_ITEMS)
                throw new PlyDataParseException(s.Position, "Encountered property value with " + n + " items.  Max number of supported items for property values is " + MAX_LIST_ITEMS);

            ReadBytes(s, _Buffer, 0, _NativeTypeSize * n);
            Array result = Array.CreateInstance(_NativeType, n);
            for (int i = 0; i < n; i++)
            {
                result.SetValue(Parse(_Buffer, i * _NativeTypeSize, this.DataType), i);
            }

            return result;
        }

        public override string HeaderDeclaration
        {
            get { return "property list " + _CountType.ToHeaderString() + ' ' + DataType.ToHeaderString() + ' ' + Name; }
        }

        public override void WriteValue(object value, Stream s)
        {
            Array items = (Array)value;
            WriteBytes((byte)items.Length, _CountType, s);
            foreach (object item in items)
                WriteBytes(item, DataType, s);
        }
    }
}
