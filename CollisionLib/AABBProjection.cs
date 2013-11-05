using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace CollisionLib
{
    public class AABBProjection : Drawable
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

        public static AABBProjectionSegment[] GetAdjacentSegments(AABBProjectionSegment segment)
        {
            switch(segment)
            {
                case AABBProjectionSegment.enTopLeft:
                    {
                        return new AABBProjectionSegment[] { AABBProjectionSegment.enTopRight, AABBProjectionSegment.enBottomLeft };
                    }
                case AABBProjectionSegment.enTopRight:
                    {
                        return new AABBProjectionSegment[] { AABBProjectionSegment.enTopLeft, AABBProjectionSegment.enBottomRight };
                    }
                case AABBProjectionSegment.enBottomLeft:
                    {
                        return new AABBProjectionSegment[] { AABBProjectionSegment.enTopLeft, AABBProjectionSegment.enBottomRight };
                    }
                case AABBProjectionSegment.enBottomRight:
                    {
                        return new AABBProjectionSegment[] { AABBProjectionSegment.enTopRight, AABBProjectionSegment.enBottomLeft };
                    }
            }           

            return null;
        }

        public static AABB.AABBSide GetSideFromProjectionSegments(AABBProjectionSegment segment1, AABBProjectionSegment segment2)
        {
            switch (segment1)
            {
                case AABBProjectionSegment.enTopLeft:
                    {
                        switch (segment2)
                        {
                            case AABBProjectionSegment.enTopRight:
                                return AABB.AABBSide.enTop;
                            case AABBProjectionSegment.enBottomLeft:
                                return AABB.AABBSide.enLeft;
                        }
                        break;
                    }
                case AABBProjectionSegment.enTopRight:
                    {
                        switch (segment2)
                        {
                            case AABBProjectionSegment.enTopLeft:
                                return AABB.AABBSide.enTop;
                            case AABBProjectionSegment.enBottomRight:
                                return AABB.AABBSide.enRight;
                        }
                        break;
                    }
                case AABBProjectionSegment.enBottomLeft:
                    {
                        switch (segment2)
                        {
                            case AABBProjectionSegment.enBottomRight:
                                return AABB.AABBSide.enBottom;
                            case AABBProjectionSegment.enTopLeft:
                                return AABB.AABBSide.enLeft;
                        }
                        break;
                    }
                case AABBProjectionSegment.enBottomRight:
                    {
                        switch (segment2)
                        {
                            case AABBProjectionSegment.enTopRight:
                                return AABB.AABBSide.enRight;
                            case AABBProjectionSegment.enBottomLeft:
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
                foreach (LineSegment segment in PathSegments)
                {
                    segment.SetColor(color);
                }
            }
        }

        public AABBProjection(AABB start, SFML.Window.Vector2f movement)
        {
            Start = start;
            End = new AABB(new SFML.Window.Vector2f(start.Position.X + movement.X, start.Position.Y + movement.Y), start.Extents.X, start.Extents.Y, Start.Color);

            this.color = Color.White;

            PathSegments = new LineSegment[4];
            PathSegments[(int)AABBProjectionSegment.enTopLeft] = new LineSegment(Start.Position, End.Position);
            PathSegments[(int)AABBProjectionSegment.enTopRight] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enRight].Start, End.Sides[(int)AABB.AABBSide.enRight].Start);
            PathSegments[(int)AABBProjectionSegment.enBottomLeft] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enLeft].Start, End.Sides[(int)AABB.AABBSide.enLeft].Start);
            PathSegments[(int)AABBProjectionSegment.enBottomRight] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enBottom].Start, End.Sides[(int)AABB.AABBSide.enBottom].Start);
        }

        public AABBProjection(AABB start, SFML.Window.Vector2f movement, SFML.Graphics.Color color)
        {
            Start = start;
            End = new AABB(new SFML.Window.Vector2f(start.Position.X + movement.X, start.Position.Y + movement.Y), start.Extents.X, start.Extents.Y, Start.Color);

            this.color = color;

            PathSegments = new LineSegment[4];
            PathSegments[(int)AABBProjectionSegment.enTopLeft] = new LineSegment(Start.Position, End.Position, color);
            PathSegments[(int)AABBProjectionSegment.enTopRight] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enRight].Start, End.Sides[(int)AABB.AABBSide.enRight].Start, color);
            PathSegments[(int)AABBProjectionSegment.enBottomLeft] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enLeft].Start, End.Sides[(int)AABB.AABBSide.enLeft].Start, color);
            PathSegments[(int)AABBProjectionSegment.enBottomRight] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enBottom].Start, End.Sides[(int)AABB.AABBSide.enBottom].Start, color);
        }

        /*public class ProjectionCollisionResult
        {
            public SFML.Window.Vector2f CollisionPoint { get; set; }
            public AABBProjectionSegment 
        }*/

        public class AABBProjectionCollisionResult : Drawable
        {
            private CollisionLib.Shapes.XShape xshape;
            public AABBProjectionSegment LocalSide { get; set; }
            public AABBProjectionSegment OtherSide { get; set; }
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

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (LineSegment segment in PathSegments)
            {
                target.Draw(segment, states);
            }
            target.Draw(End, states);
            target.Draw(Start, states);
        }
    }
}
