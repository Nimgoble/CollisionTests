using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CollisionLib;
using SFML.Graphics;

namespace TestDrawingApp
{
    public class Tests
    {
        private RenderWindow window;
        public Tests(RenderWindow window)
        {
            this.window = window;
        }

        public void TestLineCollisionTrue()
        {
            LineSegment seg1 = new LineSegment(new SFML.Window.Vector2f(0.0f, 0.0f), new SFML.Window.Vector2f(5.0f, 5.0f));
            LineSegment seg2 = new LineSegment(new SFML.Window.Vector2f(5.0f, 0.0f), new SFML.Window.Vector2f(0.0f, 5.0f));

            SFML.Window.Vector2f[] results = null;

            //Assert.IsTrue(seg1.CollidesWith(seg2, out results));

            window.Draw(seg1);
            window.Draw(seg2);

            string debug = String.Empty;
        }

        public void TestLineCollisionFalse()
        {
            LineSegment seg1 = new LineSegment(new SFML.Window.Vector2f(0.0f, 0.0f), new SFML.Window.Vector2f(5.0f, 5.0f));
            LineSegment seg2 = new LineSegment(new SFML.Window.Vector2f(5.0f, 0.0f), new SFML.Window.Vector2f(10.0f, 0.0f));

            SFML.Window.Vector2f[] results = null;

            //Assert.IsFalse(seg1.CollidesWith(seg2, out results));

            window.Draw(seg1);
            window.Draw(seg2);

            string debug = String.Empty;
        }

        public void TestLineCollisionOverlap()
        {
            LineSegment seg1 = new LineSegment(new SFML.Window.Vector2f(0.0f, 0.0f), new SFML.Window.Vector2f(5.0f, 5.0f));
            LineSegment seg2 = new LineSegment(new SFML.Window.Vector2f(2.5f, 2.5f), new SFML.Window.Vector2f(7.5f, 7.5f));

            SFML.Window.Vector2f[] results = null;
            bool collideResult = seg1.CollidesWith(seg2, out results);
            //Assert.IsTrue((collideResult && results.Length > 1));

            window.Draw(seg1);
            window.Draw(seg2);

            string debug = String.Empty;
        }

        public void TestAABBOverlapTrue()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(20.0f, 20.0f), 10.0f, 10.0f);
            AABB box2 = new AABB(new SFML.Window.Vector2f(25.0f, 25.0f), 10.0f, 10.0f);

            bool rtn1 = box1.Overlaps(box2);
            bool rtn2 = box2.Overlaps(box1);

            window.Draw(box1);
            window.Draw(box2);

            //Assert.IsTrue(rtn1 && rtn2);
        }

        public void TestAABBOverlapFalse()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(20.0f, 20.0f), 10.0f, 10.0f);
            AABB box2 = new AABB(new SFML.Window.Vector2f(40.0f, 40.0f), 10.0f, 10.0f);

            bool rtn1 = box1.Overlaps(box2);
            bool rtn2 = box2.Overlaps(box1);

            window.Draw(box1);
            window.Draw(box2);

            //Assert.IsFalse(rtn1 && rtn2);
        }

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

            window.Draw(projection1);
            window.Draw(projection2);

            //Assert.IsTrue(collisions);
        }

        public void AABBProjectionTestA()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(50.0f, 300.0f), 50.0f, 50.0f, Color.Blue);
            AABB box2 = new AABB(new SFML.Window.Vector2f(650.0f, 100.0f), 50.0f, 50.0f, Color.Magenta);

            AABBProjection projection1 = new AABBProjection(box1, new SFML.Window.Vector2f(251.0f, -225.0f), Color.Cyan);
            AABBProjection projection2 = new AABBProjection(box2, new SFML.Window.Vector2f(-300.0f, 0.0f));

            TestProjections(projection1, projection2);
        }

        public void AABBProjectionTestB()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(150.0f, 100.0f), 50.0f, 50.0f, Color.Blue);
            AABB box2 = new AABB(new SFML.Window.Vector2f(150.0f, 300.0f), 50.0f, 50.0f, Color.Magenta);

            AABBProjection projection1 = new AABBProjection(box1, new SFML.Window.Vector2f(300.0f, 300.0f), Color.Cyan);
            AABBProjection projection2 = new AABBProjection(box2, new SFML.Window.Vector2f(300.0f, -300.0f));

            TestProjections(projection1, projection2);
        }

        public void AABBProjectionTestC()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(150.0f, 100.0f), 50.0f, 50.0f, Color.Blue);
            AABB box2 = new AABB(new SFML.Window.Vector2f(175.0f, 350.0f), 50.0f, 50.0f, Color.Magenta);

            AABBProjection projection1 = new AABBProjection(box1, new SFML.Window.Vector2f(300.0f, 300.0f), Color.Cyan);
            AABBProjection projection2 = new AABBProjection(box2, new SFML.Window.Vector2f(400.0f, -300.0f));

            TestProjections(projection1, projection2);
        }

        public void AABBProjectionTestD()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(150.0f, 100.0f), 50.0f, 50.0f, Color.Blue);
            AABB box2 = new AABB(new SFML.Window.Vector2f(350.0f, 100.0f), 50.0f, 50.0f, Color.Magenta);

            AABBProjection projection1 = new AABBProjection(box1, new SFML.Window.Vector2f(500.0f, 250.0f), Color.Cyan);
            AABBProjection projection2 = new AABBProjection(box2, new SFML.Window.Vector2f(-250.0f, 300.0f));

            TestProjections(projection1, projection2);
        }

        public void AABBProjectionTestE()
        {
            AABB box1 = new AABB(new SFML.Window.Vector2f(300.0f, 550.0f), 50.0f, 50.0f, Color.Blue);
            AABB box2 = new AABB(new SFML.Window.Vector2f(340.0f, 300.0f), 50.0f, 50.0f, Color.Magenta);

            AABBProjection projection1 = new AABBProjection(box1, new SFML.Window.Vector2f(0.0f, -249.0f), Color.Cyan);
            AABBProjection projection2 = new AABBProjection(box2, new SFML.Window.Vector2f(1.0f, -1.0f));

            TestProjections(projection1, projection2);
        }

        private void TestProjections(AABBProjection projection1, AABBProjection projection2)
        {
            List<AABBProjection.AABBProjectionCollisionResult> results = null;

            bool collisions = projection1.CollidesWith(projection2, out results);

            List<AABBProjection.AABBProjectionCollisionResult> sorted = null;
            if (collisions)
            {
                sorted = results.OrderBy(x => x.Length).ToList();

                AABBProjection.AABBProjectionCollisionResult shortestResult = sorted[0];

                AABBProjection.AABBProjectionSegmentEnum[] adjacentSegments = AABBProjection.GetAdjacentSegments(shortestResult.LocalSide);

                List<AABBProjection.AABBProjectionCollisionResult> adjacentSegmentResults = sorted.Where(x => x.OtherSide == shortestResult.OtherSide && adjacentSegments.Contains(x.LocalSide)).OrderBy(x => x.Length).ToList();

                if (adjacentSegmentResults.Count > 0)
                {
                    float shortestLength = adjacentSegmentResults[0].Length;
                    //There could be multiples if we hit a corner
                    adjacentSegmentResults = adjacentSegmentResults.Where(x => x.Length == shortestLength).ToList();

                    foreach (AABBProjection.AABBProjectionCollisionResult collisionResult in adjacentSegmentResults)
                    {
                        AABB.AABBSide collisionSide = AABBProjection.GetSideFromProjectionSegments(shortestResult.LocalSide, collisionResult.LocalSide);

                        projection1.Start.Sides[(int)collisionSide].SetColor(Color.Red);

                        AABB.AABBSide otherCollisionSide = AABB.GetOppositeSide(collisionSide);

                        projection2.Start.Sides[(int)otherCollisionSide].SetColor(Color.Red);
                    }
                }

                //Draw our shortest line
                CollisionLib.Shapes.XShape shortestStartingPointA = new CollisionLib.Shapes.XShape(projection1.PathSegments[shortestResult.LocalSide].Path.Start, 3.0f, Color.Yellow);
                window.Draw(shortestStartingPointA);

                CollisionLib.Shapes.XShape shortestStartingPointB = new CollisionLib.Shapes.XShape(projection2.PathSegments[shortestResult.OtherSide].Path.Start, 3.0f, Color.Yellow);
                window.Draw(shortestStartingPointB);
            }

            //Draw stuff
            window.Draw(projection1);
            window.Draw(projection2);

            if (sorted != null)
            {
                foreach (AABBProjection.AABBProjectionCollisionResult result in sorted)
                    window.Draw(result);
            }
        }
    }
}
