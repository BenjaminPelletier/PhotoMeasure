using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    /// <summary>
    /// Represents the information contained in the ASCII header of a PLY file
    /// </summary>
    class PlyHeader
    {
        public PlyFormat Format;

        /// <summary>
        /// The list of elements defined by the header (Key), and how many elements of each type there are (Value)
        /// </summary>
        public List<KeyValuePair<PlyElement, int>> ElementSets;

        /// <summary>
        /// The offset from the beginning of the file in bytes where the non-header data content may be found
        /// </summary>
        public long DataStart;

        public PlyHeader(Stream s)
        {
            // Set up reading infrastructure
            var sr = new StreamReader(s);
            int lineNumber = 1;
            Func<string> readLine = () =>
            {
                string result = sr.ReadLine();
                lineNumber++;
                DataStart += result.Length + 1; //Assumes lines are always terminated only with \n and never \r\n (as per spec)
                return result;
            };

            // Check magic number
            string firstLine = readLine();
            if (firstLine != "ply")
                throw new PlyHeaderParseException(lineNumber, "No 'ply' header found; instead found '" + firstLine + "'");

            // Read format
            string formatLine = readLine();
            if (!formatLine.StartsWith("format "))
                throw new PlyHeaderParseException(lineNumber, "Expected 'format XXX', instead found '" + formatLine + "'");
            Format = PlyFormatConversions.FromString(formatLine);
            if (Format != PlyFormat.BinaryLittleEndian10)
                throw new NotSupportedException("This importer does not yet support the format '" + formatLine.Substring("format ".Length) + "', but such support would be relatively easy to add");

            // Read elements
            ElementSets = new List<KeyValuePair<PlyElement, int>>();
            string line = readLine();
            while (line != "end_header")
            {
                if (line.StartsWith("element "))
                {
                    // Parse element definition
                    string[] cols = line.Split(' ');
                    if (cols.Length != 3)
                        throw new PlyHeaderParseException(lineNumber, "Expected element definition 'element NAME COUNT'; found instead '" + line + "'");
                    string elementName = cols[1];
                    int n = int.Parse(cols[2]);
                    var element = new PlyElement(elementName);

                    // Parse element properties
                    line = readLine();
                    while (line.StartsWith("property "))
                    {
                        cols = line.Split(' ');
                        if (cols.Length == 3 && cols[0] == "property")
                        {
                            // Parse a single-valued property
                            if (!PlyPropertyTypes.IsValid(cols[1]))
                                throw new PlyHeaderParseException(lineNumber, "Unsupported property data type '" + cols[1] + "'");
                            element.AddProperty(new PlyPropertySingle(cols[2], PlyPropertyTypes.Parse(cols[1])));
                        }
                        else if (cols.Length == 5 && cols[0] == "property" && cols[1] == "list")
                        {
                            // Parse a list-based property
                            if (!PlyPropertyTypes.IsValid(cols[3]))
                                throw new PlyHeaderParseException(lineNumber, "Unsupported property list item data type '" + cols[3] + "'");
                            if (!PlyPropertyTypes.IsValid(cols[2]))
                                throw new PlyHeaderParseException(lineNumber, "Unsupported property list count data type '" + cols[2] + "'");
                            element.AddProperty(new PlyPropertyList(cols[4], PlyPropertyTypes.Parse(cols[3]), PlyPropertyTypes.Parse(cols[2])));
                        }
                        else
                        {
                            throw new PlyHeaderParseException(lineNumber, "Expected property definition 'property TYPE NAME' or 'property list COUNTTYPE VALUETYPE NAME'; found instead '" + line + "'");
                        }

                        line = readLine();
                    }

                    // Add element definition
                    ElementSets.Add(new KeyValuePair<PlyElement, int>(element, n));
                }
                else if (line.StartsWith("comment"))
                {
                    // Skip comment lines
                    line = readLine();
                }
                else
                {
                    throw new PlyHeaderParseException(lineNumber, "Expected directive: 'element NAME COUNT', 'comment XXX', or 'end_header'; found instead '" + line + "' (");
                }
            }

            //TODO: verify that content just preceding DataStart in stream is "end_header\n"
        }

        public int CountOf(string elementName)
        {
            return ElementSets.Where(kvp => kvp.Key.Name == elementName).Select(kvp => kvp.Value).First();
        }

        public PlyHeader(List<KeyValuePair<PlyElement, int>> elementSets)
        {
            Format = PlyFormat.BinaryLittleEndian10;
            ElementSets = elementSets;
            //Note that DataStart is not populated with this constructor
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ply\n");
            sb.Append(Format.ToHeaderString() + '\n');
            foreach (var kvp in ElementSets)
            {
                sb.Append("element " + kvp.Key.Name + ' ' + kvp.Value + '\n');
                foreach (var p in kvp.Key.Properties)
                    sb.Append(p.HeaderDeclaration + '\n');
            }
            sb.Append("end_header\n");
            return sb.ToString();
        }
    }
}
