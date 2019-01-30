using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    public class PlyHeaderParseException : Exception
    {
        public int Line;

        public PlyHeaderParseException(int line, string message)
            : base(message)
        {
            this.Line = line;
        }

        public PlyHeaderParseException(int line, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Line = line;
        }
    }
}
