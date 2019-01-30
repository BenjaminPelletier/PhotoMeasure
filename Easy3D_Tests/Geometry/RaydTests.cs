using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy3D.Geometry;
using OpenTK;
using System.Collections.Generic;

namespace Easy3D_Tests.Geometry
{
    [TestClass]
    public class RaydTests
    {
        static Random random = new Random(1234);
        static Func<double, double, double, Vector3d> V = (x, y, z) => new Vector3d(x, y, z);
        static Func<double> r = () => random.NextDouble();
        static Func<Vector3d> rV = () => V(r(), r(), r());

        [TestMethod]
        public void NearestPoints_CommonOriginSimpleDirections()
        {
            var r1 = new Rayd(Vector3d.Zero, Vector3d.UnitX);
            var r2 = new Rayd(Vector3d.Zero, Vector3d.UnitY);
            var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
            var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
            Assert.AreEqual(p2.Length, 0, 1e-6);
            Assert.AreEqual(pn.Length, 0, 1e-6);
        }

        [TestMethod]
        public void NearestPoints_TranslatedOriginsSimpleDirections()
        {
            var r1 = new Rayd(Vector3d.Zero, Vector3d.UnitX);
            r1.Origin -= r1.Direction;
            var r2 = new Rayd(Vector3d.Zero, Vector3d.UnitY);
            r2.Origin -= r2.Direction;
            var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
            var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
            Assert.AreEqual(p2.Length, 0, 1e-6);
            Assert.AreEqual(pn.Length, 0, 1e-6);
        }

        [TestMethod]
        public void NearestPoints_CommonOriginRandomDirections()
        {
            var r1 = new Rayd(Vector3d.Zero, rV());
            var r2 = new Rayd(Vector3d.Zero, rV());
            var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
            var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
            Assert.AreEqual(p2.Length, 0, 1e-6);
            Assert.AreEqual(pn.Length, 0, 1e-6);
        }

        [TestMethod]
        public void NearestPoints_OffsetOriginsOrthogonalDirections()
        {
            var r1 = new Rayd(Vector3d.UnitZ, Vector3d.UnitX);
            var r2 = new Rayd(-Vector3d.UnitZ, Vector3d.UnitY);
            var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
            var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
            Assert.AreEqual(p2.Length, 0, 1e-6);
            Assert.AreEqual(pn.Length, 0, 1e-6);
        }

        [TestMethod]
        public void NearestPoints_OffsetOriginsSimpleDirections()
        {
            var r1 = new Rayd(Vector3d.UnitZ, Vector3d.UnitX);
            var r2 = new Rayd(-Vector3d.UnitZ, V(10, 1, 0));
            var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
            var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
            Assert.AreEqual(p2.Length, 0, 1e-6);
            Assert.AreEqual(pn.Length, 0, 1e-6);
        }

        [TestMethod]
        public void NearestPoints_OffsetOriginsRandomXYPlaneDirections()
        {
            var r1 = new Rayd(Vector3d.UnitZ, V(r(), r(), 0));
            var r2 = new Rayd(-Vector3d.UnitZ, V(r(), r(), 0));
            var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
            var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
            Assert.AreEqual(p2.Length, 0, 1e-6);
            Assert.AreEqual(pn.Length, 0, 1e-6);
        }

        [TestMethod]
        public void NearestPoints_TranslatedOriginsRandomDirections()
        {
            var r1 = new Rayd(Vector3d.Zero, rV());
            double t1 = r();
            r1.Origin -= t1 * r1.Direction;
            var r2 = new Rayd(Vector3d.Zero, rV());
            double t2 = r();
            r2.Origin -= t2 * r2.Direction;
            var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
            var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
            Assert.AreEqual(p2.Length, 0, 1e-6);
            Assert.AreEqual(pn.Length, 0, 1e-6);
        }

        [TestMethod]
        public void NearestPoints_SpecificScenarios()
        {
            Vector3d[,] rays =
            {
                {  V(0.399080979357977, 0.895899465724779, 0.319202938731389), V(0.946737533876085, 0.339436024585476, 0.948778240917613), V(0.807991890147325, 0.520730946921153, 0.643958064096029), V(0.312558948207907, 0.417377764553473, 0.868940052049672) },
            };
            for (int i = 0; i < rays.GetLength(0); i++)
            {
                var r1 = new Rayd(rays[i, 0], rays[i, 1]);
                var r2 = new Rayd(rays[i, 2], rays[i, 3]);
                var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
                var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
                Assert.AreEqual((pn - p2).Length, 0, 1e-6, "Scenario " + i);
            }
        }

        [TestMethod]
        public void NearestPoints_RandomOriginsAndDirections()
        {
            var r1 = new Rayd(rV(), rV());
            var r2 = new Rayd(rV(), rV());
            var p2 = Rayd.NearestPoints(r1, r2).DistanceAlong(0.5);
            var pn = Rayd.NearestPoint(new Rayd[] { r1, r2 });
            string message = "Rays <" + r1.Origin.ToString() + " " + r1.Direction.ToString() + "> and <" + r2.Origin.ToString() + " " + r2.Direction.ToString() + ">";
            Assert.AreEqual((pn - p2).Length, 0, 1e-6, message);
        }
    }
}
