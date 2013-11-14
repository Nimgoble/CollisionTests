using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VectorClass;
using CollisionLib;

namespace CollisionLineTests
{
    [TestFixture]
    public class TestFixture1
    {
        [Test]
        public void TestLineCollisionTrue()
        {
            LineSegment seg1 = new LineSegment(new SFML.Window.Vector2f(0.0f, 0.0f), new SFML.Window.Vector2f(5.0f, 5.0f));
            LineSegment seg2 = new LineSegment(new SFML.Window.Vector2f(5.0f, 0.0f), new SFML.Window.Vector2f(0.0f, 5.0f));

            SFML.Window.Vector2f[] results = null;

            Assert.IsTrue(seg1.CollidesWith(seg2, out results));

            string debug = String.Empty;
        }

        [Test]
        public void TestLineCollisionFalse()
        {
            LineSegment seg1 = new LineSegment(new SFML.Window.Vector2f(0.0f, 0.0f), new SFML.Window.Vector2f(5.0f, 5.0f));
            LineSegment seg2 = new LineSegment(new SFML.Window.Vector2f(5.0f, 0.0f), new SFML.Window.Vector2f(10.0f, 0.0f));

            SFML.Window.Vector2f[] results = null;

            Assert.IsFalse(seg1.CollidesWith(seg2, out results));

            string debug = String.Empty;
        }

        [Test]
        public void TestLineCollisionOverlap()
        {
            LineSegment seg1 = new LineSegment(new SFML.Window.Vector2f(0.0f, 0.0f), new SFML.Window.Vector2f(5.0f, 5.0f));
            LineSegment seg2 = new LineSegment(new SFML.Window.Vector2f(2.5f, 2.5f), new SFML.Window.Vector2f(7.5f, 7.5f));

            SFML.Window.Vector2f[] results = null;
            bool collideResult = seg1.CollidesWith(seg2, out results);
            Assert.IsTrue((collideResult && results.Length > 1));

            string debug = String.Empty;
        }

        [Test]
        public void TestAABBOverlapTrue()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(20.0f, 20.0f), 10.0f, 10.0f);
            AABB box2 = new AABB(new SFML.Window.Vector2f(25.0f, 25.0f), 10.0f, 10.0f);

            bool rtn1 = box1.Overlaps(box2);
            bool rtn2 = box2.Overlaps(box1);

            Assert.IsTrue(rtn1 && rtn2);
        }

        [Test]
        public void TestAABBOverlapFalse()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(20.0f, 20.0f), 10.0f, 10.0f);
            AABB box2 = new AABB(new SFML.Window.Vector2f(40.0f, 40.0f), 10.0f, 10.0f);

            bool rtn1 = box1.Overlaps(box2);
            bool rtn2 = box2.Overlaps(box1);

            Assert.IsFalse(rtn1 && rtn2);
        }

        [Test]
        public void TestAABBProjectionDictionary()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(50.0f, 100.0f), 10.0f, 10.0f);
            AABB box2 = new AABB(new SFML.Window.Vector2f(50.0f, 50.0f), 10.0f, 10.0f);

            AABBProjection projection1 = new AABBProjection(box1, new SFML.Window.Vector2f(30.0f, -30.0f));
            AABBProjection projection2 = new AABBProjection(box2, new SFML.Window.Vector2f(30.0f, 30.0f));

            Dictionary<AABBProjection.AABBProjectionSegmentEnum, 
                    Dictionary<AABBProjection.AABBProjectionSegmentEnum, 
                        List<SFML.Window.Vector2f>>> results = null;

            bool collisions = projection1.CollidesWith(projection2, out results);

            Assert.IsTrue(collisions);
        }

        [Test]
        public void TestAABBProjectionList()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(50.0f, 80.0f), 10.0f, 10.0f);
            AABB box2 = new AABB(new SFML.Window.Vector2f(50.0f, 50.0f), 10.0f, 10.0f);

            AABBProjection projection1 = new AABBProjection(box1, new SFML.Window.Vector2f(30.0f, -30.0f));
            AABBProjection projection2 = new AABBProjection(box2, new SFML.Window.Vector2f(30.0f, 30.0f));

            List<AABBProjection.AABBProjectionCollisionResult> results = null;

            bool collisions = projection1.CollidesWith(projection2, out results);

            List<AABBProjection.AABBProjectionCollisionResult> sorted = results.OrderBy(x => x.Length).ToList();

            Assert.IsTrue(collisions);

            string debug = String.Empty;
        }
    }
}
