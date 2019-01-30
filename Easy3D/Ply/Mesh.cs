using Easy3D.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Ply
{
    /// <summary>
    /// Represents a set of triangular faces defined by vertices and, optionally, vertex normals
    /// </summary>
    public class Mesh : IDisposable
    {
        #region Geometric data

        public Vector3[] Vertices { get; private set; }
        public uint[] VertexColors { get; private set; }
        public Vector3[] VertexNormals { get; private set; }
        public uint[] Faces { get; private set; }

        private object _GeoDataBaton = new object();

        #endregion

        #region GL buffer handles and information

        private bool _InGLMemory = false;

        private int _VertexId;
        private int _VertexStride;

        private int _ColorId;
        private int _ColorStride;

        private int _FaceId;

        private int _NormalId;
        private int _NormalStride;

        #endregion

        protected Mesh() { }

        public Mesh(Vector3[] vertices, uint[] colors, uint[] faces, Vector3[] normals = null)
        {
            if (vertices == null || colors == null || faces == null)
                throw new ArgumentNullException();
            if (colors.Length != vertices.Length)
                throw new ArgumentException("Length of colors array must match length of vertices array; instead vertices.Length=" + vertices.Length + " and colors.length=" + colors.Length);
            if (faces.Length % 3 != 0)
                throw new ArgumentException("Provided faces array must contain n*3 vertex indices defining n triangles; " + faces.Length + " is not divisible by 3");
            int nVertices = vertices.Length;
            if (faces.Any(f => f >= nVertices))
                throw new ArgumentException("Face indices must not equal or exceed the number of vertices provided; observed face index of " + faces.Max() + ", but only " + nVertices + " vertices are defined");
            if (normals != null && normals.Length != vertices.Length)
                throw new ArgumentException("There must be exactly one normal per vertex");

            Vertices = vertices;
            VertexColors = colors;
            Faces = faces;
            VertexNormals = normals;
        }

        public void Attach(Mesh other)
        {
            lock (_GeoDataBaton)
            {
                UnloadFromGL();
                Vector3[] newVertices = new Vector3[Vertices.Length + other.Vertices.Length];
                uint[] newColors = new uint[VertexColors.Length + other.VertexColors.Length];
                uint[] newFaces = new uint[Faces.Length + other.Faces.Length];
                Vector3[] newNormals = null;
                if (VertexNormals != null && other.VertexNormals != null)
                {
                    newNormals = new Vector3[VertexNormals.Length + other.VertexNormals.Length];
                }
                else if (VertexNormals == null && other.VertexNormals == null)
                {
                    newNormals = other.VertexNormals;
                }
                throw new NotImplementedException();
            }
        }

        public void WriteToGL()
        {
            lock (_GeoDataBaton)
            {
                if (_InGLMemory)
                    return;
                _InGLMemory = true;

                WriteVerticesToGL();
                WriteColorsToGL();
                WriteFacesToGL();
                WriteNormalsToGL();
            }
        }

        private void WriteVerticesToGL()
        {
            int expectedBufferSize;
            int glBufferSize;
            
            _VertexStride = BlittableValueType.StrideOf(Vertices);
            GL.GenBuffers(1, out _VertexId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * _VertexStride), Vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out glBufferSize);
            expectedBufferSize = Vertices.Length * _VertexStride;
            if (expectedBufferSize != glBufferSize)
                throw new Exception("Vertex data transfer error: Expected " + expectedBufferSize + " bytes in buffer, observed " + glBufferSize + " bytes");
        }

        private void WriteColorsToGL()
        {
            int expectedBufferSize;
            int glBufferSize;

            _ColorStride = BlittableValueType.StrideOf(VertexColors);
            GL.GenBuffers(1, out _ColorId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _ColorId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexColors.Length * _ColorStride), VertexColors, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out glBufferSize);
            expectedBufferSize = VertexColors.Length * BlittableValueType.StrideOf(VertexColors);
            if (expectedBufferSize != glBufferSize)
                throw new Exception("Color data transfer error: Expected " + expectedBufferSize + " bytes in buffer, observed " + glBufferSize + " bytes");
        }

        private void WriteFacesToGL()
        {
            int expectedBufferSize;
            int glBufferSize;

            GL.GenBuffers(1, out _FaceId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _FaceId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out glBufferSize);
            expectedBufferSize = Faces.Length * sizeof(int);
            if (expectedBufferSize != glBufferSize)
                throw new Exception("Face data transfer error: Expected " + expectedBufferSize + " bytes in buffer, observed " + glBufferSize + " bytes");
        }

        private void WriteNormalsToGL()
        {
            int expectedBufferSize;
            int glBufferSize;

            if (VertexNormals != null)
            {
                _NormalStride = BlittableValueType.StrideOf(VertexNormals);
                GL.GenBuffers(1, out _NormalId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _NormalId);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexNormals.Length * _NormalStride), VertexNormals, BufferUsageHint.StaticDraw);
                GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out glBufferSize);
                expectedBufferSize = VertexNormals.Length * _NormalStride;
                if (expectedBufferSize != glBufferSize)
                    throw new Exception("Normal data transfer error: Expected " + expectedBufferSize + " bytes in buffer, observed " + glBufferSize + " bytes");
            }
        }

        public void Draw()
        {
            lock (_GeoDataBaton)
            {
                if (!_InGLMemory)
                    WriteToGL();

                GL.Disable(EnableCap.CullFace);

                GL.EnableClientState(ArrayCap.ColorArray);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.EnableClientState(ArrayCap.IndexArray);

                // Bind the vertex array
                GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexId);
                GL.VertexPointer(3, VertexPointerType.Float, _VertexStride, IntPtr.Zero);

                // Bind the normal array, if available
                if (VertexNormals != null)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _NormalId);
                    GL.NormalPointer(NormalPointerType.Float, _NormalStride, IntPtr.Zero);
                }

                // Bind the color array
                GL.BindBuffer(BufferTarget.ArrayBuffer, _ColorId);
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, _ColorStride, IntPtr.Zero);

                // Bind the elements array
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _FaceId);
                GL.DrawElements(PrimitiveType.Triangles, Faces.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }
        }

        public void UnloadFromGL()
        {
            lock (_GeoDataBaton)
            {
                if (!_InGLMemory)
                    return;

                _InGLMemory = false;
                GL.DeleteBuffer(_VertexId);
                GL.DeleteBuffer(_ColorId);
                GL.DeleteBuffer(_FaceId);
                if (VertexNormals != null)
                    GL.DeleteBuffer(_NormalId);
            }
        }

        #region Mutations

        public int FixWindingOrder()
        {
            lock (_GeoDataBaton)
            {
                if (VertexNormals == null)
                    return 0;

                int nFixed = 0;

                for (int f = 0; f < Faces.Length; f += 3)
                {
                    uint f0 = Faces[f];
                    uint f1 = Faces[f + 1];
                    uint f2 = Faces[f + 2];
                    Vector3 faceNormal = VertexNormals[f0] + VertexNormals[f1] + VertexNormals[f2];

                    Vector3 v0 = Vertices[f0];
                    Vector3 v1 = Vertices[f1];
                    Vector3 v2 = Vertices[f2];
                    Vector3 windingNormal = Vector3.Cross(v1 - v0, v2 - v1);

                    if (Vector3.Dot(faceNormal, windingNormal) < 0)
                    {
                        Faces[f + 1] = f2;
                        Faces[f + 2] = f1;
                        nFixed++;
                    }
                }

                if (_InGLMemory)
                {
                    GL.DeleteBuffer(_FaceId);
                    WriteFacesToGL();
                }

                return nFixed;
            }
        }

        public void Translate(Vector3 v)
        {
            lock (_GeoDataBaton)
            {
                for (int i = 0; i < Vertices.Length; i++)
                    Vertices[i] += v;

                if (_InGLMemory)
                {
                    GL.DeleteBuffer(_VertexId);
                    WriteVerticesToGL();
                }
            }
        }

        public void Rotate(Quaternion q)
        {
            lock (_GeoDataBaton)
            {
                for (int i = 0; i < Vertices.Length; i++)
                    Vertices[i] = Vector3.Transform(Vertices[i], q);

                if (_InGLMemory)
                {
                    GL.DeleteBuffer(_VertexId);
                    WriteVerticesToGL();
                }

                if (VertexNormals != null)
                {
                    for (int i = 0; i < VertexNormals.Length; i++)
                        VertexNormals[i] = Vector3.Transform(VertexNormals[i], q);

                    if (_InGLMemory)
                    {
                        GL.DeleteBuffer(_NormalId);
                        WriteNormalsToGL();
                    }
                }
            }
        }

        /// <summary>
        /// Changes the color of all vertices
        /// </summary>
        /// <param name="f">
        /// Function that produces the new color for the vertex.  Arguments are:
        /// Vector3: Vertex location
        /// Vector3: Vertex normal
        /// uint: Vertex index (what faces refer to)
        /// Color: Current color of vertex
        /// </param>
        public void Recolor(Func<Vector3, Vector3, uint, Color, Color> f)
        {
            lock (_GeoDataBaton)
            {
                // TODO: consider moving the recoloring operation out of the lock
                for (uint i = 0; i < VertexColors.Length; i++)
                    VertexColors[i] = f(Vertices[i], VertexNormals[i], i, VertexColors[i].ToGlColor()).ToGlColor();

                if (_InGLMemory)
                {
                    GL.DeleteBuffer(_ColorId);
                    WriteColorsToGL();
                }
            }
        }

        #endregion

        #region Properties

        public Vector3 ComputeCenter()
        {
            lock (_GeoDataBaton)
            {
                var center = new Vector3(0, 0, 0);

                foreach (Vector3 v in Vertices)
                    center += v;

                return center / Vertices.Length;
            }
        }

        public double ComputeExtent(Vector3 refPt)
        {
            lock (_GeoDataBaton)
            {
                double extent = 0;

                foreach (Vector3 v in Vertices)
                {
                    double newExtent = (v - refPt).Length;
                    if (newExtent > extent)
                        extent = newExtent;
                }

                return extent;
            }
        }

        public PointOnFace GetIntersectedFace(Rayd r)
        {
            int closest = -1;
            Vector3 intersection = Vector3.Zero;
            double closestDistance2 = double.PositiveInfinity;

            for (int f = 0; f < Faces.Length; f += 3)
            {
                Vector3 c0 = Vertices[Faces[f]];
                Vector3 c1 = Vertices[Faces[f + 1]];
                Vector3 c2 = Vertices[Faces[f + 2]];

                Vector3 side1 = c1 - c0;
                Vector3 side2 = c2 - c0;
                Vector3 normal = Vector3.Cross(side1, side2); //The face plane normal is side1 x side2
                Planed face = new Planed(c0, normal); //Define the infinite plane that contains this face
                Vector3d p = r.IntersectionWith(face); //Find the intersection of the given ray with the infinite plane
                Matrix4d m = new Matrix4d
                ( //Define a new coordinate system where corner 0 is the origin, the triangle in the XY plane with side 1 as X and side 2 as Y, and the normal is Z (all points in the face plane should have z=0)
                    side1.X, side1.Y, side1.Z, 0,
                    side2.X, side2.Y, side2.Z, 0,
                    normal.X, normal.Y, normal.Z, 0,
                    c0.X, c0.Y, c0.Z, 1
                );
                Vector3d barycentric = Vector3d.Transform(p, m.Inverted());
                if (barycentric.X >= 0 && barycentric.Y >= 0 && barycentric.X + barycentric.Y <= 1)
                {
                    double newDistance2 = Vector3d.Dot(p - r.Origin, r.Direction);
                    if (newDistance2 > 0 && newDistance2 < closestDistance2)
                    {
                        closest = f;
                        intersection = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
                        closestDistance2 = newDistance2;
                    }
                }
            }

            if (closest > -1)
                return new PointOnFace(closest, intersection);
            else
                return null;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            UnloadFromGL();
        }

        #endregion
    }

    public class PointOnFace
    {
        public readonly int FaceIndex;
        public readonly Vector3 Point;

        public PointOnFace(int faceIndex, Vector3 point)
        {
            this.FaceIndex = faceIndex;
            this.Point = point;
        }
    }
}
