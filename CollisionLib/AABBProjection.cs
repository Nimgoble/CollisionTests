using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionLib
{
    public class AABBProjection
    {
        public enum AABBProjectionSegment
        {
            enTopLeft = 0,
            enTopRight = 1,
            enBottomLeft = 2,
            enBottomRight = 3
        };

        public AABB Start { get; set; }
        public AABB End { get; set; }
        public LineSegment[] PathSegments { get; set; }


        public AABBProjection(AABB start, SFML.Window.Vector2f movement)
        {
            Start = start;
            End = new AABB(new SFML.Window.Vector2f(start.Position.X + movement.X, start.Position.Y + movement.Y), start.Extents.X, start.Extents.Y);

            PathSegments = new LineSegment[4];
            PathSegments[(int)AABBProjectionSegment.enTopLeft] = new LineSegment(Start.Position, End.Position);
            PathSegments[(int)AABBProjectionSegment.enTopRight] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enRight].Start, End.Sides[(int)AABB.AABBSide.enRight].Start);
            PathSegments[(int)AABBProjectionSegment.enBottomLeft] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enLeft].Start, End.Sides[(int)AABB.AABBSide.enLeft].Start);
            PathSegments[(int)AABBProjectionSegment.enBottomRight] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enBottom].Start, End.Sides[(int)AABB.AABBSide.enBottom].Start);
        }

        /*public class ProjectionCollisionResult
        {
            public SFML.Window.Vector2f CollisionPoint { get; set; }
            public AABBProjectionSegment 
        }*/

        public class AABBProjectionCollisionResult
        {
            public AABBProjectionSegment LocalSide { get; set; }
            public AABBProjectionSegment OtherSide { get; set; }
            public float Length { get; set; }
            public SFML.Window.Vector2f CollisionPoint { get; set; }
        }

        public bool CollidesWith(AABBProjection other, out Dictionary<AABBProjectionSegment, Dictionary<AABBProjectionSegment, List<SFML.Window.Vector2f>>> results)
        {
            results = new Dictionary<AABBProjectionSegment, Dictionary<AABBProjectionSegment, List<SFML.Window.Vector2f>>>();

            if (Start.Overlaps(other.Start))
                return true;

            bool collisions = false;
            
            for (int currentLocalSegment = 0; currentLocalSegment < 4; currentLocalSegment++)
            {
                LineSegment currentLocal = PathSegments[currentLocalSegment];
                results[(AABBProjectionSegment)currentLocalSegment] = new Dictionary<AABBProjectionSegment, List<SFML.Window.Vector2f>>();
                for (int currentOtherSegment = 0; currentOtherSegment < 4; currentOtherSegment++)
                {
                    LineSegment currentOther = other.PathSegments[currentOtherSegment];
                    SFML.Window.Vector2f[] segmentIntersectionResults = null;
                    results[(AABBProjectionSegment)currentLocalSegment][(AABBProjectionSegment)currentOtherSegment] = new List<SFML.Window.Vector2f>();
                    if (currentLocal.CollidesWith(currentOther, out segmentIntersectionResults) && collisions == false)
                        collisions = true;

                    results[(AABBProjectionSegment)currentLocalSegment][(AABBProjectionSegment)currentOtherSegment].AddRange(segmentIntersectionResults);
                }
            }

            return collisions;
        }

        public bool CollidesWith(AABBProjection other, out List<AABBProjectionCollisionResult> results)
        {
            results = new List<AABBProjectionCollisionResult>();

            if (Start.Overlaps(other.Start))
                return true;

            bool collisions = false;

            for (int currentLocalSegmentCounter = 0; currentLocalSegmentCounter < 4; currentLocalSegmentCounter++)
            {
                LineSegment currentLocal = PathSegments[currentLocalSegmentCounter];
                AABBProjectionSegment currentLocalSegment = (AABBProjectionSegment)currentLocalSegmentCounter;
                for (int currentOtherSegmentCounter = 0; currentOtherSegmentCounter < 4; currentOtherSegmentCounter++)
                {
                    LineSegment currentOther = other.PathSegments[currentOtherSegmentCounter];
                    SFML.Window.Vector2f[] segmentIntersectionResults = null;
                    AABBProjectionSegment currentOtherSegment = (AABBProjectionSegment)currentOtherSegmentCounter;

                    if (currentLocal.CollidesWith(currentOther, out segmentIntersectionResults) && collisions == false)
                        collisions = true;

                    foreach (SFML.Window.Vector2f intersection in segmentIntersectionResults)
                    {
                        AABBProjectionCollisionResult aabbResult = new AABBProjectionCollisionResult();
                        aabbResult.CollisionPoint = intersection;
                        aabbResult.Length = DistanceBetweenTwoPoints(currentLocal.Start, intersection);
                        aabbResult.LocalSide = currentLocalSegment;
                        aabbResult.OtherSide = currentOtherSegment;

                        results.Add(aabbResult);
                    }
                }
            }

            return collisions;
        }

        private float DistanceBetweenTwoPoints(SFML.Window.Vector2f a, SFML.Window.Vector2f b)
        {
            float distanceX = b.X - a.X;
            float distanceY = b.Y - a.Y;

            return (float)Math.Sqrt((Double)(distanceX * distanceX) + (Double)(distanceY * distanceY));
        }
    }
}
