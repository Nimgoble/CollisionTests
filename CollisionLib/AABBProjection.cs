using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace CollisionLib
{
    public class AABBProjection : Drawable
    {
        public enum AABBProjectionSegmentEnum
        {
            enTopLeft = 0,
            enTopRight = 1,
            enBottomLeft = 2,
            enBottomRight = 3
        };

        public class AABBProjectionSegment
        {
            public AABBProjectionSegment()
            {
            }
            public AABBProjectionSegment(LineSegment path, AABBProjectionSegmentEnum segmentEnum)
            {
                Path = path;
                SegmentEnum = segmentEnum;
            }
            public LineSegment Path { get; set; }
            public AABBProjectionSegmentEnum SegmentEnum { get; set; }
            public AABBProjectionSegmentJoiner Next { get; set; }
            public AABBProjectionSegmentJoiner Previous { get; set; }
        };

        public class AABBProjectionSegmentJoiner
        {
            public AABBProjectionSegment Segment { get; set; }
            public AABB.AABBSide Side { get; set; }
        }

        public AABB Start { get; set; }
        public AABB End { get; set; }
        public Dictionary<AABBProjectionSegmentEnum, AABBProjectionSegment> PathSegments { get; set; }

        public static AABBProjectionSegmentEnum[] GetAdjacentSegments(AABBProjectionSegmentEnum segment)
        {
            switch(segment)
            {
                case AABBProjectionSegmentEnum.enTopLeft:
                    {
                        return new AABBProjectionSegmentEnum[] { AABBProjectionSegmentEnum.enTopRight, AABBProjectionSegmentEnum.enBottomLeft };
                    }
                case AABBProjectionSegmentEnum.enTopRight:
                    {
                        return new AABBProjectionSegmentEnum[] { AABBProjectionSegmentEnum.enTopLeft, AABBProjectionSegmentEnum.enBottomRight };
                    }
                case AABBProjectionSegmentEnum.enBottomLeft:
                    {
                        return new AABBProjectionSegmentEnum[] { AABBProjectionSegmentEnum.enTopLeft, AABBProjectionSegmentEnum.enBottomRight };
                    }
                case AABBProjectionSegmentEnum.enBottomRight:
                    {
                        return new AABBProjectionSegmentEnum[] { AABBProjectionSegmentEnum.enTopRight, AABBProjectionSegmentEnum.enBottomLeft };
                    }
            }           

            return null;
        }

        public static AABB.AABBSide GetSideFromProjectionSegments(AABBProjectionSegmentEnum segment1, AABBProjectionSegmentEnum segment2)
        {
            switch (segment1)
            {
                case AABBProjectionSegmentEnum.enTopLeft:
                    {
                        switch (segment2)
                        {
                            case AABBProjectionSegmentEnum.enTopRight:
                                return AABB.AABBSide.enTop;
                            case AABBProjectionSegmentEnum.enBottomLeft:
                                return AABB.AABBSide.enLeft;
                        }
                        break;
                    }
                case AABBProjectionSegmentEnum.enTopRight:
                    {
                        switch (segment2)
                        {
                            case AABBProjectionSegmentEnum.enTopLeft:
                                return AABB.AABBSide.enTop;
                            case AABBProjectionSegmentEnum.enBottomRight:
                                return AABB.AABBSide.enRight;
                        }
                        break;
                    }
                case AABBProjectionSegmentEnum.enBottomLeft:
                    {
                        switch (segment2)
                        {
                            case AABBProjectionSegmentEnum.enBottomRight:
                                return AABB.AABBSide.enBottom;
                            case AABBProjectionSegmentEnum.enTopLeft:
                                return AABB.AABBSide.enLeft;
                        }
                        break;
                    }
                case AABBProjectionSegmentEnum.enBottomRight:
                    {
                        switch (segment2)
                        {
                            case AABBProjectionSegmentEnum.enTopRight:
                                return AABB.AABBSide.enRight;
                            case AABBProjectionSegmentEnum.enBottomLeft:
                                return AABB.AABBSide.enBottom;
                        }
                        break;
                    }
            }

            throw new Exception("Segments do not form a side!");
        }

        private SFML.Graphics.Color color;
        public SFML.Graphics.Color Color
        {
            get { return color; }
            set
            {
                color = value;
                foreach (AABBProjectionSegment segment in PathSegments.Values)
                {
                    segment.Path.SetColor(color);
                }
            }
        }

        public AABBProjection(AABB start, SFML.Window.Vector2f movement)
        {
            Start = start;
            End = new AABB(new SFML.Window.Vector2f(start.Position.X + movement.X, start.Position.Y + movement.Y), start.Extents.X, start.Extents.Y, Start.Color);

            this.color = Color.White;

            InitializeSegments();
        }

        public AABBProjection(AABB start, SFML.Window.Vector2f movement, SFML.Graphics.Color color)
        {
            Start = start;
            End = new AABB(new SFML.Window.Vector2f(start.Position.X + movement.X, start.Position.Y + movement.Y), start.Extents.X, start.Extents.Y, Start.Color);

            this.color = color;

            InitializeSegments();
        }

        private void InitializeSegments()
        {
            PathSegments = new Dictionary<AABBProjectionSegmentEnum, AABBProjectionSegment>();

            PathSegments[AABBProjectionSegmentEnum.enTopLeft] = new AABBProjectionSegment(new LineSegment(Start.Position, End.Position, Color),
                                                                                                AABBProjectionSegmentEnum.enTopLeft);
            PathSegments[AABBProjectionSegmentEnum.enTopRight] = new AABBProjectionSegment(new LineSegment(Start.Sides[(int)AABB.AABBSide.enRight].Start,
                                                                                                                End.Sides[(int)AABB.AABBSide.enRight].Start,
                                                                                                                Color),
                                                                                                AABBProjectionSegmentEnum.enTopRight);
            PathSegments[AABBProjectionSegmentEnum.enBottomLeft] = new AABBProjectionSegment(new LineSegment(Start.Sides[(int)AABB.AABBSide.enLeft].Start,
                                                                                                                    End.Sides[(int)AABB.AABBSide.enLeft].Start,
                                                                                                                    Color),
                                                                                                    AABBProjectionSegmentEnum.enBottomLeft);
            PathSegments[AABBProjectionSegmentEnum.enBottomRight] = new AABBProjectionSegment(new LineSegment(Start.Sides[(int)AABB.AABBSide.enBottom].Start,
                                                                                                                    End.Sides[(int)AABB.AABBSide.enBottom].Start,
                                                                                                                    Color),
                                                                                                    AABBProjectionSegmentEnum.enBottomRight);

            PathSegments[AABBProjectionSegmentEnum.enTopLeft].Next = new AABBProjectionSegmentJoiner() { Segment = PathSegments[AABBProjectionSegmentEnum.enTopRight], Side = AABB.AABBSide.enTop };
            PathSegments[AABBProjectionSegmentEnum.enTopLeft].Previous = new AABBProjectionSegmentJoiner() { Segment = PathSegments[AABBProjectionSegmentEnum.enBottomLeft], Side = AABB.AABBSide.enLeft };

            PathSegments[AABBProjectionSegmentEnum.enTopRight].Next = new AABBProjectionSegmentJoiner() { Segment = PathSegments[AABBProjectionSegmentEnum.enBottomRight], Side = AABB.AABBSide.enRight };
            PathSegments[AABBProjectionSegmentEnum.enTopRight].Previous = new AABBProjectionSegmentJoiner() { Segment = PathSegments[AABBProjectionSegmentEnum.enTopLeft], Side = AABB.AABBSide.enTop };

            PathSegments[AABBProjectionSegmentEnum.enBottomRight].Next = new AABBProjectionSegmentJoiner() { Segment = PathSegments[AABBProjectionSegmentEnum.enBottomLeft], Side = AABB.AABBSide.enBottom};
            PathSegments[AABBProjectionSegmentEnum.enBottomRight].Previous = new AABBProjectionSegmentJoiner() { Segment = PathSegments[AABBProjectionSegmentEnum.enTopRight], Side = AABB.AABBSide.enRight };

            PathSegments[AABBProjectionSegmentEnum.enBottomLeft].Next = new AABBProjectionSegmentJoiner() { Segment = PathSegments[AABBProjectionSegmentEnum.enTopLeft], Side = AABB.AABBSide.enLeft };
            PathSegments[AABBProjectionSegmentEnum.enBottomLeft].Previous = new AABBProjectionSegmentJoiner() { Segment = PathSegments[AABBProjectionSegmentEnum.enBottomRight], Side = AABB.AABBSide.enBottom };
        }

        public class AABBProjectionCollisionResult : Drawable
        {
            private CollisionLib.Shapes.XShape xshape;
            public AABBProjectionSegmentEnum LocalSide { get; set; }
            public AABBProjectionSegmentEnum OtherSide { get; set; }
            public float Length { get; set; }
            private SFML.Window.Vector2f collisionPoint;
            public SFML.Window.Vector2f CollisionPoint 
            {
                get { return collisionPoint; }
                set
                {
                    collisionPoint = value;
                    xshape = new Shapes.XShape(collisionPoint, 3.0f);
                }
            }

            public void Draw(RenderTarget target, RenderStates states)
            {
                target.Draw(xshape, states);
            }
        }

        /*
         * Collision should go like this:
         * -Check if both starts are overlapping each other
         * --If so: collision
         * -Check if the projection segments collide
         * --If so: 
         * ---Find out the earliest collision point
         * ----Project each AABB to that point in time and see if they overlap
         * -----If not: no collision
         * -----If so:
         * ------Determine which sides will be colliding
         * 
         * What collision information we want:
         * -What sides collided
         * -What type of collision(predictive, absolute, none)
         * -What objects collided
         * -The time at which they collided
         */ 

        public bool CollidesWith(AABBProjection other, out Dictionary<AABBProjectionSegmentEnum, Dictionary<AABBProjectionSegmentEnum, List<SFML.Window.Vector2f>>> results)
        {
            results = new Dictionary<AABBProjectionSegmentEnum, Dictionary<AABBProjectionSegmentEnum, List<SFML.Window.Vector2f>>>();

            if (Start.Overlaps(other.Start))
                return true;

            bool collisions = false;

            foreach (AABBProjectionSegment localSegment in PathSegments.Values)
            {
                results[localSegment.SegmentEnum] = new Dictionary<AABBProjectionSegmentEnum, List<SFML.Window.Vector2f>>();

                foreach (AABBProjectionSegment otherSegment in other.PathSegments.Values)
                {
                    SFML.Window.Vector2f[] segmentIntersectionResults = null;
                    results[localSegment.SegmentEnum][otherSegment.SegmentEnum] = new List<SFML.Window.Vector2f>();
                    if (localSegment.Path.CollidesWith(otherSegment.Path, out segmentIntersectionResults) && collisions == false)
                        collisions = true;

                    results[localSegment.SegmentEnum][otherSegment.SegmentEnum].AddRange(segmentIntersectionResults);
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

            foreach (AABBProjectionSegment localSegment in PathSegments.Values)
            {
                foreach (AABBProjectionSegment otherSegment in other.PathSegments.Values)
                {
                    SFML.Window.Vector2f[] segmentIntersectionResults = null;

                    if (localSegment.Path.CollidesWith(otherSegment.Path, out segmentIntersectionResults) && collisions == false)
                        collisions = true;

                    foreach (SFML.Window.Vector2f intersection in segmentIntersectionResults)
                    {
                        AABBProjectionCollisionResult aabbResult = new AABBProjectionCollisionResult();
                        aabbResult.CollisionPoint = intersection;
                        aabbResult.Length = DistanceBetweenTwoPoints(localSegment.Path.Start, intersection);
                        aabbResult.LocalSide = localSegment.SegmentEnum;
                        aabbResult.OtherSide = otherSegment.SegmentEnum;

                        results.Add(aabbResult);
                    }
                }
            }

            return collisions;
        }

        /*public List<CollisionResults> CollidesWith(AABBProjection other)
        {
            List<CollisionResults> results = new List<CollisionResults>();

            if (Start.Overlaps(other.Start))
            {
                results.Add(new CollisionResults() { Type = CollisionType.enAbsolute });
                return results;
            }

            if (End.Overlaps(other.End))
            {
                foreach (AABBProjectionSegment segment in PathSegments.Values)
                {
                    foreach (LineSegment otherSegment in other.End.Sides)
                    {
                        SFML.Window.Vector2f[] collisionPoints;
                        bool collides = segment.Path.CollidesWith(otherSegment, out collisionPoints);
                        if (collides)
                        {

                        }
                    }
                }
            }
            return results;
        }*/

        private float DistanceBetweenTwoPoints(SFML.Window.Vector2f a, SFML.Window.Vector2f b)
        {
            float distanceX = b.X - a.X;
            float distanceY = b.Y - a.Y;

            return (float)Math.Sqrt((Double)(distanceX * distanceX) + (Double)(distanceY * distanceY));
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (AABBProjectionSegment segment in PathSegments.Values)
            {
                target.Draw(segment.Path, states);
            }
            target.Draw(End, states);
            target.Draw(Start, states);
        }
    }
}
