using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VectorClass;
namespace CollisionLineTests
{
    [TestFixture]
    public class TestFixture1
    {
        [Test]
        public void TestLineCollisionTrue()
        {
            LineSegment seg1 = new LineSegment(new Vector2D_Dbl(0.0, 0.0), new Vector2D_Dbl(5.0, 5.0));
            LineSegment seg2 = new LineSegment(new Vector2D_Dbl(5.0, 0.0), new Vector2D_Dbl(0.0, 5.0));

            Vector2D_Dbl[] results = null;

            Assert.IsTrue(seg1.CollidesWith(seg2, out results));

            string debug = String.Empty;
        }

        [Test]
        public void TestLineCollisionFalse()
        {
            LineSegment seg1 = new LineSegment(new Vector2D_Dbl(0.0, 0.0), new Vector2D_Dbl(5.0, 5.0));
            LineSegment seg2 = new LineSegment(new Vector2D_Dbl(5.0, 0.0), new Vector2D_Dbl(10.0, 0.0));

            Vector2D_Dbl[] results = null;

            Assert.IsFalse(seg1.CollidesWith(seg2, out results));

            string debug = String.Empty;
        }

        [Test]
        public void TestLineCollisionOverlap()
        {
            LineSegment seg1 = new LineSegment(new Vector2D_Dbl(0.0, 0.0), new Vector2D_Dbl(5.0, 5.0));
            LineSegment seg2 = new LineSegment(new Vector2D_Dbl(2.5, 2.5), new Vector2D_Dbl(7.5, 7.5));

            Vector2D_Dbl[] results = null;
            bool collideResult = seg1.CollidesWith(seg2, out results);
            Assert.IsTrue((collideResult && results.Length > 1));

            string debug = String.Empty;
        }

        [Test]
        public void TestAABBOverlapTrue()
        {
            AABB box1 = new AABB(new Vector2D_Dbl(20.0, 20.0), 10.0, 10.0);
            AABB box2 = new AABB(new Vector2D_Dbl(25.0, 25.0), 10.0, 10.0);

            bool rtn1 = box1.Overlaps(box2);
            bool rtn2 = box2.Overlaps(box1);

            Assert.IsTrue(rtn1 && rtn2);
        }
    }
}
