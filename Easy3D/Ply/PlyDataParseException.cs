using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    class PlyDataParseException : Exception
    {
        public long Position;

        public PlyDataParseException(long position, string message)
            : base(message)
        {
            this.Position = position;
        }
    }
}
