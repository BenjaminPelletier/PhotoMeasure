using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    public class PlyFile
    {
        /// <summary>
        /// Describes the data contained in this PLY file
        /// </summary>
        private PlyHeader _Header;

        /// <summary>
        /// The full set of data (apart from header) contained in the PLY file;
        /// this is the content that would be in binary in binary-formatted PLY files.
        /// </summary>
        /// <remarks>
        /// Key is the name of the set of same-typed elements (like "vertex" and "vertex_indices").
        /// Value is the set of properties for that element set.  In that key-value pair,
        ///     Key is the name of the property in the element type;
        ///     Value is the array of values for that property for each element.
        /// </remarks>
        private Dictionary<string, Dictionary<string, Array>> _ColumnData = new Dictionary<string, Dictionary<string, Array>>();

        public PlyFile(Stream s)
        {
            _Header = new PlyHeader(s);
            s.Seek(_Header.DataStart, SeekOrigin.Begin);

            _ColumnData = new Dictionary<string, Dictionary<string, Array>>();
            foreach (var kvp in _Header.ElementSets)
            {
                // Initialize property columns for this element set
                var propertyData = new Dictionary<string, Array>();
                foreach (PlyProperty p in kvp.Key.Properties)
                {
                    Array dataColumn = p.CreateArray(kvp.Value);
                    propertyData[p.Name] = dataColumn;
                }

                // Read all the elements for this element set
                PlyProperty[] indexedProperties = kvp.Key.Properties.ToArray();
                Array[] indexedPropertyData = propertyData.Values.ToArray();
                for (int i = 0; i < kvp.Value; i++)
                {
                    for (int j = 0; j < indexedProperties.Length; j++)
                    {
                        indexedPropertyData[j].SetValue(indexedProperties[j].ReadValue(s), i);
                    }
                }

                // Store this element set's column data
                _ColumnData[kvp.Key.Name] = propertyData;
            }
        }

        public PlyFile(Mesh m)
        {
            var elementSets = new List<KeyValuePair<PlyElement, int>>();

            var vertexElement = new PlyElement("vertex");
            new string[] { "x", "y", "z", "nx", "ny", "nz" }.ForEach(prop => vertexElement.AddProperty(new PlyPropertySingle(prop, PlyPropertyType.Float)));
            new string[] { "red", "green", "blue", "alpha" }.ForEach(prop => vertexElement.AddProperty(new PlyPropertySingle(prop, PlyPropertyType.UChar)));
            elementSets.Add(new KeyValuePair<PlyElement, int>(vertexElement, m.Vertices.Length));

            var faceElement = new PlyElement("face");
            faceElement.AddProperty(new PlyPropertyList("vertex_indices", PlyPropertyType.Int, PlyPropertyType.UChar));
            elementSets.Add(new KeyValuePair<PlyElement, int>(faceElement, m.Faces.Length / 3));

            _Header = new PlyHeader(elementSets);

            // This section could be sped up by iterating only once by index and sharing a single color conversion
            var vertex = new Dictionary<string, Array>();
            vertex["x"] = m.Vertices.Select(v => v.X).ToArray();
            vertex["y"] = m.Vertices.Select(v => v.Y).ToArray();
            vertex["z"] = m.Vertices.Select(v => v.Z).ToArray();
            vertex["nx"] = m.VertexNormals.Select(v => v.X).ToArray();
            vertex["ny"] = m.VertexNormals.Select(v => v.Y).ToArray();
            vertex["nz"] = m.VertexNormals.Select(v => v.Z).ToArray();
            vertex["red"] = m.VertexColors.Select(c => Color.FromArgb((int)c).B).ToArray();
            vertex["green"] = m.VertexColors.Select(c => Color.FromArgb((int)c).G).ToArray();
            vertex["blue"] = m.VertexColors.Select(c => Color.FromArgb((int)c).R).ToArray();
            vertex["alpha"] = m.VertexColors.Select(c => Color.FromArgb((int)c).A).ToArray();
            _ColumnData["vertex"] = vertex;

            int[][] indices = new int[m.Faces.Length / 3][];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = new int[] { (int)m.Faces[i * 3], (int)m.Faces[i * 3 + 1], (int)m.Faces[i * 3 + 2] };
            _ColumnData["face"] = new Dictionary<string, Array>() { { "vertex_indices", indices } };
        }

        public Mesh ToMesh()
        {
            int nVertices = _Header.CountOf("vertex");

            Dictionary<string, Array> v = _ColumnData["vertex"];

            float[] x = (float[])v["x"];
            float[] y = (float[])v["y"];
            float[] z = (float[])v["z"];
            byte[] r = (byte[])v["red"];
            byte[] g = (byte[])v["green"];
            byte[] b = (byte[])v["blue"];
            byte[] a;
            if (v.ContainsKey("alpha"))
            {
                a = (byte[])v["alpha"];
            }
            else
            {
                a = new byte[r.Length];
                for (int i = 0; i < a.Length; i++)
                    a[i] = 255;
            }

            var vertices = new Vector3[nVertices];
            var colors = new uint[nVertices];
            for (int i = 0; i < nVertices; i++)
            {
                vertices[i].X = x[i];
                vertices[i].Y = y[i];
                vertices[i].Z = z[i];
                colors[i] = (uint)a[i] << 24 | (uint)b[i] << 16 | (uint)g[i] << 8 | (uint)r[i];
            }

            Vector3[] normals = null;
            if (v.ContainsKey("nx") && v.ContainsKey("ny") && v.ContainsKey("nz"))
            {
                float[] nx = (float[])v["nx"];
                float[] ny = (float[])v["ny"];
                float[] nz = (float[])v["nz"];
                normals = new Vector3[nVertices];
                for (int i = 0; i < nVertices; i++)
                {
                    normals[i].X = nx[i];
                    normals[i].Y = ny[i];
                    normals[i].Z = nz[i];
                }
            }

            int nFaces = _Header.CountOf("face");
            int[][] triangles = (int[][])_ColumnData["face"]["vertex_indices"];
            uint[] faces = new uint[nFaces * 3];
            for (int t = 0; t < nFaces; t++)
            {
                int[] triangle = triangles[t];
                if (triangle.Length != 3)
                    throw new Exception("Only triangles are supported; encountered face with " + triangle.Length + " vertices");
                int i = 3 * t;
                faces[i] = (uint)triangle[0];
                faces[i + 1] = (uint)triangle[1];
                faces[i + 2] = (uint)triangle[2];
            }

            return new Mesh(vertices, colors, faces, normals);
        }

        public void Save(Stream s)
        {
            byte[] header = ASCIIEncoding.ASCII.GetBytes(_Header.ToString());
            s.Write(header, 0, header.Length);
            foreach (var kvp in _Header.ElementSets)
            {
                Dictionary<string, Array> element = _ColumnData[kvp.Key.Name];
                PlyProperty[] indexedProperties = kvp.Key.Properties.ToArray();
                System.Collections.IEnumerator[] indexedEnumerators = indexedProperties.Select(p => element[p.Name].GetEnumerator()).ToArray();
                indexedEnumerators.ForEach(e => e.MoveNext());
                for (int i = 0; i < kvp.Value; i++)
                {
                    for (int j = 0; j < indexedProperties.Length; j++)
                    {
                        indexedProperties[j].WriteValue(indexedEnumerators[j].Current, s);
                        indexedEnumerators[j].MoveNext();
                    }
                }
            }
        }
    }
}
